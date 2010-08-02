#!/bin/sh
echo $0 | cut --delimiter='/' --fields=-`echo $0 | awk -F '/' '{print NF-1 }'` | xargs -i java -jar '{}'/Ares.Controller.jar

