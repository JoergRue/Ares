About Ares Controller
------------------------
Ares Controller controls the Ares Player, also remotely through a network.

Licence
-------
Ares Controller is open source using the General Public Licence. See licence.txt

Version History
---------------
See ChangeLog.txt

Build
-----
You need to reference all libraries from the libraries directory of the binary installation of Ares Controller. Under linux:

mkdir classes
javac -source 1.6 -target 1.6 -d classes -cp libraries/jdic.jar:libraries/skinlf.jar:libraries/synthetica.jar:libraries/syntheticaBlueIce.jar:libraries/syntheticaBlueSteel.jar:libraries/syntheticaGreenDream.jar:libraries/syntheticaSilverMoon.jar:libraries/syntheticawalnut.jar `find source/ares -name "*.java"`

mv Manifest classes/
cd classes
jar cfm Ares.Controller.jar Manifest ares

