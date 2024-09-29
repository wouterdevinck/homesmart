all: build

.PHONY: all
all:
	./build.sh all

.PHONY: version
version:
	./build.sh version
	
.PHONY: build
build:
	./build.sh build

.PHONY: run
run:
	./build.sh run
	
.PHONY: push
push:
	./build.sh push

.PHONY: lf
lf:
	./build.sh lf
	
.PHONY: swa
swa:
	./build.sh swa

.PHONY: bundle
bundle:
	./build.sh bundle
	
.PHONY: factory
factory:
	./build.sh factory
	
.PHONY: qemu
qemu:
	./build.sh qemu