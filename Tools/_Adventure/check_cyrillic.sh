#!/usr/bin/sh
set -xue
(! git grep -IP -e "([a-zA-Z][а-яА-Я]|[а-яА-Я][a-zA-Z])" --and --not -e "(\\\n[а-яА-Я]|Zа|LARPеров|Jaсkal|flazeг)" "${GITHUB_BASE_REF}")
(! git -c core.quotepath=off ls-tree --full-tree -r -t --name-only --full-name "${GITHUB_BASE_REF}" | grep -P "[а-яА-Я]" | grep -v "Resources/Textures/Effects/text.rsi")
