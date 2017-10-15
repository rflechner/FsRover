#!/usr/bin/env bash

set -eu
set -o pipefail

cd `dirname $0`

FAKE_EXE=packages/FAKE/tools/FAKE.exe
NUGET_EXE=.nuget/NuGet.exe

FSIARGS=""
OS=${OS:-"unknown"}
if [[ "$OS" != "Windows_NT" ]]
then
  FSIARGS="--fsiargs -d:MONO"
fi

function run() {
  if [[ "$OS" != "Windows_NT" ]]
  then
    mono "$@"
  else
    "$@"
  fi
}

if [[ "$OS" != "Windows_NT" ]] &&
       [ ! -e ~/.config/.mono/certs ]
then
  mozroots --import --sync --quiet
fi

cd src
run ../$NUGET_EXE restore
cd ..
run $NUGET_EXE Install FAKE -OutputDirectory packages -ExcludeVersion

[ ! -e build.fsx ] && run $PAKET_EXE update
[ ! -e build.fsx ] && run $FAKE_EXE init.fsx
run $FAKE_EXE "$@" $FSIARGS build.fsx


