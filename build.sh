#!/bin/bash
set -e

EDGEOS_VERSION="0.7.0-3-ga753405"

VERSION="$(git describe --tags --dirty)"
TAG="wouterdevinck/homesmart:$VERSION"

SCRIPT_DIR="$( cd "$( dirname "$0" )" && pwd )"

EDGEOS_DIR="$SCRIPT_DIR/edgeos"
EDGEOS_DOCKER_TAG_BUNDLER="wouterdevinck/edgeos-bundler:$EDGEOS_VERSION"
EDGEOS_BUNDLER_ARGS="-v $EDGEOS_DIR:/workdir -v /var/run/docker.sock:/var/run/docker.sock -u $(id -u $USER):$(getent group docker | cut -d: -f3)"
EDGEOS_BUNDLER="docker run --rm $EDGEOS_BUNDLER_ARGS $EDGEOS_DOCKER_TAG_BUNDLER"

function printUsage {
  echo "Usage: $0 all|version|build|run|push|lf|swa|bundle|factory"
  echo ""
  echo "   all     - Build, push and deploy."
  echo "   version - Print current version number."
  echo "   build   - Build the Docker image."
  echo "   run     - Run Docker image locally."
  echo "   push    - Push to Docker Hub."
  echo "   lf      - Fix line endings."
  echo "   swa     - Build and deploy Azure static web application."
  echo "   bundle  - EdgeOS upgrade package."
  echo "   factory - Build EdgeOS factory image."
  echo ""
}

case $1 in

"all")
  $0 build
  $0 push
  $0 deploy
  ;;

"version")
  echo "$TAG"
  ;;

"build")

  # Build image for all architectures
  docker build -t $TAG-amd64 .
  docker build --build-arg ARCH=arm32v7 -t $TAG-arm32v7 .
  docker build --build-arg ARCH=arm64v8 -t $TAG-arm64v8 .

  ;;

"run")

  # Run the amd64 variant locally
  docker run --rm -it -p 5000:5000 -e HOME_CONFIG=/config/config-local.yaml -v $(pwd)/config:/config $TAG-amd64

  ;;

"push")

  # Push all image versions
  docker push $TAG-amd64
  docker push $TAG-arm32v7
  docker push $TAG-arm64v8

  # Combine all into a multi-arch image and push
  docker manifest create $TAG --amend $TAG-amd64 --amend $TAG-arm32v7 --amend $TAG-arm64v8
  docker manifest push $TAG

  ;;

"lf")

  # Fix line endings
  find . -type f -name '*.cs' -not -path "*/obj/*" -exec dos2unix '{}' +
  find . -type f -name '*.csproj' -not -path "*/obj/*" -exec dos2unix '{}' +
  find . -type f -name '*.sln' -not -path "*/obj/*" -exec dos2unix '{}' +
  find . -type f -name '*.vue' -not -path "*/obj/*" -exec dos2unix '{}' +

  ;;

"swa")

  # Build and deploy Azure Static Web Application (SWA)
  swa build
  swa deploy --deployment-token $2 --env production --api-location src/backend/Home.Remote.Functions/bin/Release/net7.0/publish

  ;;

"bundle"|"factory")

  # Fill in version numbers
  jq \
    --arg v $VERSION \
    --arg ev $EDGEOS_VERSION \
    '.app.version=$v|.edgeos.version=$ev' \
    $EDGEOS_DIR/manifest-template.json \
    > $EDGEOS_DIR/manifest.json
  TAG=$TAG yq e \
    '.services.homesmart.image=strenv(TAG)' \
    $EDGEOS_DIR/docker-compose-template.yml \
    > $EDGEOS_DIR/docker-compose.yml

  ;;&

"bundle")

  # Build EdgeOS upgrade package
  $EDGEOS_BUNDLER create-upgrade

  ;;

"factory")

  # Build full EdgeOS disk image
  $EDGEOS_BUNDLER create-image

  ;;

*)
  printUsage
  ;;

esac