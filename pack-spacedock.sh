#!/usr/bin/env bash

# not needed for while
exit 0

# see http://redsymbol.net/articles/unofficial-bash-strict-mode/
set -euo pipefail
IFS=$'\n\t'
source ./CONFIG.inc

clean() {
	rm -fR $FILE
	if [ ! -d Archive ] ; then
		mkdir Archive
	fi
}

pwd=$(pwd)
FILE=${pwd}/Archive/$PACKAGE-$VERSION${PROJECT_STATE}-SpaceDock.zip
echo $FILE
clean
zip -r $FILE ./GameData/* -x ".*"
set +e
zip -r $FILE ./PluginData/* -x ".*"
zip -r $FILE ./Extras/* -x ".*"
zip $FILE INSTALL.md

mkdir -p $pwd/bin/GameData/$TARGETBINDIR
cat GameData/$TARGETDIR/KSP-WhoAmI.cfg | sed 's/ckan_ready = False/ckan_ready = True/g' > $pwd/bin/GameData/$TARGETDIR/KSP-WhoAmI.cfg
cp ${pwd}/bin/ReleaseCkan/Scale.dll $pwd/bin/GameData/$TARGETBINDIR
pushd $pwd/bin
zip -u $FILE GameData/WhoAmI/Plugins/WhoAmI.dll
#zip -u $FILE GameData/WhoAmI/KSP-WhoAmI.cfg
popd

zip -d $FILE "__MACOSX/*" "**/.DS_Store" || echo ""

set -e
cd $pwd
