Full Fiction - manual install
=============================

ENGLISH

Install
1. Close the game.
2. In Steam: right-click Split Fiction > Manage > Browse local files.
   This opens your "Split Fiction" folder. Go one level up, to
   ...\steamapps\common
3. Drag the "Split Fiction" folder from this archive into "common".
   When Windows asks, choose to merge folders / replace files.
   You should end up with:
     common\Split Fiction\Split\Binaries\Win64\dwmapi.dll
     common\Split Fiction\Split\Binaries\Win64\ue4ss\...
4. Launch the game from Steam. Press F10 in game:
   split screen > your view fullscreen > partner's view fullscreen

Auto mode (optional)
Open Split\Binaries\Win64\ue4ss\Mods\FullFiction\Scripts\fullfiction_config.lua
in Notepad and change "false" to "true". After every cutscene and
loading screen your view goes fullscreen automatically. A bit less
predictable in rare spots; press F10 if something looks off.

Uninstall
Delete Split\Binaries\Win64\dwmapi.dll and the Split\Binaries\Win64\ue4ss
folder, or use Steam's "Verify integrity of game files".

Already have UE4SS from another mod? This package replaces its settings
with the ones tested for Split Fiction. Back up your ue4ss folder first.


РУССКИЙ

Установка
1. Закрой игру.
2. В Steam: правый клик по Split Fiction > Управление > Просмотреть
   локальные файлы. Откроется твоя папка "Split Fiction". Поднимись
   на уровень выше, в ...\steamapps\common
3. Перетащи папку "Split Fiction" из этого архива в "common".
   Когда Windows спросит, выбери объединить папки / заменить файлы.
   Должно получиться:
     common\Split Fiction\Split\Binaries\Win64\dwmapi.dll
     common\Split Fiction\Split\Binaries\Win64\ue4ss\...
4. Запускай игру из Steam. В игре жми F10:
   раздельный экран > твой персонаж > персонаж напарника

Автоматический режим (по желанию)
Открой в Блокноте Split\Binaries\Win64\ue4ss\Mods\FullFiction\Scripts\fullfiction_config.lua
и поменяй "false" на "true". После каждой катсцены и загрузки мод
сам включит твой экран. Чуть менее стабильно в редких местах,
тогда просто жми F10.

Удаление
Удали Split\Binaries\Win64\dwmapi.dll и папку Split\Binaries\Win64\ue4ss,
или в Steam: Свойства > Установленные файлы > Проверить целостность.

Уже стоит UE4SS от другого мода? Этот пакет заменит его настройки
на проверенные для Split Fiction. Сначала сделай копию папки ue4ss.
