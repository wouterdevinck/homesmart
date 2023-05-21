#!/bin/bash
set -e

TAG="wouterdevinck/homesmart:$(git describe --tags --dirty)"

function printUsage {
  echo "Usage: $0 all|version|build|run|push|lf|swa"
  echo ""
  echo "   all     - Build, push and deploy."
  echo "   version - Print current version number."
  echo "   build   - Build the Docker image."
  echo "   run     - Run Docker image locally."
  echo "   push    - Push to Docker Hub."
  echo "   lf      - Fix line endings."
  echo "   swa     - Deploy static web application."
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

  ;;

"swa")

  swa build
  swa deploy --deployment-token $2 --env production --api-location src/backend/Home.Remote.Functions/bin/Release/net7.0/publish

  ;;

*)
  printUsage
  ;;

esac