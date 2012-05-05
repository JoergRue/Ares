#!/bin/sh
BASEDIR=$(dirname $0)
cd $BASEDIR
LD_LIBRARY_PATH=. mono Ares.Player.exe $1

