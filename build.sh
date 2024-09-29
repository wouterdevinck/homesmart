#!/bin/bash
set -e

EDGEOS_VERSION="0.9.2"
EDGEOS_NAME="Homesmart"

VERSION="$(git describe --tags --dirty)"
TAG="wouterdevinck/homesmart:$VERSION"

SCRIPT_DIR="$( cd "$( dirname "$0" )" && pwd )"

EDGEOS_DIR="$SCRIPT_DIR/config"
EDGEOS_DOCKER_TAG_BUNDLER="wouterdevinck/edgeos-bundler:$EDGEOS_VERSION"
EDGEOS_BUNDLER_ARGS="-v $EDGEOS_DIR:/workdir -v /var/run/docker.sock:/var/run/docker.sock -u $(id -u $USER):$(getent group docker | cut -d: -f3)"
EDGEOS_BUNDLER="docker run --rm $EDGEOS_BUNDLER_ARGS $EDGEOS_DOCKER_TAG_BUNDLER"

QEMU_DISK_SIZE=20GB

function printUsage {
  echo "Usage: $0 all|version|build|run|push|lf|swa|bundle|factory|qemu"
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
  echo "   qemu    - Run EdgeOS image in QEMU."
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
  docker buildx build --load --build-arg ARCH=amd64 -t $TAG-amd64 .
  docker buildx build --load --build-arg ARCH=arm64v8 -t $TAG-arm64v8 .

  ;;

"run")

  # Run the amd64 variant locally
  docker run --rm -it -p 5000:5000 -e HOME_CONFIG=/config/config-local.yaml -v $(pwd)/config:/config $TAG-amd64

  ;;

"push")

  # Push all image versions
  docker push $TAG-amd64
  docker push $TAG-arm64v8

  # Combine all into a multi-arch image and push
  docker manifest create $TAG --amend $TAG-amd64 --amend $TAG-arm64v8
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

"bundle"|"factory"|"qemu")

  # Fill in version numbers
  jq \
    --arg v $VERSION \
    --arg ev $EDGEOS_VERSION \
    --arg n $EDGEOS_NAME \
    '.app.name=$n|.app.version=$v|.edgeos.version=$ev' \
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

  # Build full EdgeOS disk image for Raspberry Pi
  $EDGEOS_BUNDLER create-image rpi4

  ;;

"qemu")

  # Build full EdgeOS disk image for PC
  $EDGEOS_BUNDLER create-image pc -raw

  # Disk path
  DISK="$EDGEOS_DIR/$EDGEOS_NAME-pc-$VERSION.img"

  # Make the disk image bigger
  dd if=/dev/zero of=$DISK seek=$QEMU_DISK_SIZE obs=1MB count=0 > /dev/null 2>&1

  # If no EFI NVRAM file, copy the default one
  if [ ! -e OVMF_VARS.fd ]; then
    cp /usr/share/OVMF/OVMF_VARS.fd .
  fi

  # Run PC variant in QEMU with EFI
  qemu-system-x86_64 \
    -drive if=pflash,format=raw,readonly=on,file=/usr/share/OVMF/OVMF_CODE.fd \
    -drive if=pflash,format=raw,file=OVMF_VARS.fd \
    -drive file=$DISK,format=raw \
    -m 4G \
    -nographic \
    -cpu host \
    -smp 4 \
    -enable-kvm
    
  ;;

*)
  printUsage
  ;;

esac