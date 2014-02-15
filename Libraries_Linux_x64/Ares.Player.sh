#!/bin/sh
BASEDIR=$(dirname $0)
cd $BASEDIR
LD_LIBRARY_PATH=. mono --runtime=v4.0 Ares.Player.exe $1

