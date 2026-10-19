using System.Globalization;

namespace FullFiction.Setup
{
    internal static class UiText
    {
        public static readonly bool IsRussian = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ru";

        private static string T(string ru, string en) => IsRussian ? ru : en;

        public static string Description => T(
            "Каждый игрок может видеть свой экран целиком. В игре жми F10.",
            "Each player can see their own view fullscreen. Press F10 in game.");

        public static string AutoModeCheckBox => T(
            "Автоматический режим (наведи, чтобы узнать подробнее)",
            "Auto mode (hover for details)");

        public static string AutoModeTitle => T("Автоматический режим", "Auto mode");

        public static string AutoModeHint => T(
            "Автоматический режим: после каждой катсцены и загрузки мод сам включает\n" +
            "полный экран твоего персонажа, F10 нажимать не нужно.\n\n" +
            "Удобнее, но менее стабильно: в редких местах игра может показать\n" +
            "не то, что задумано. Тогда просто нажми F10.\n\n" +
            "Без галочки: после катсцен остаётся обычный раздельный экран,\n" +
            "полный экран включаешь сам клавишей F10.",
            "Auto mode: after every cutscene and loading screen the mod switches\n" +
            "to your character's fullscreen view by itself, no need to press F10.\n\n" +
            "More convenient but less stable: in rare spots the game may show\n" +
            "something other than intended. Just press F10 then.\n\n" +
            "Unchecked: after cutscenes you keep the regular split screen\n" +
            "and switch to fullscreen yourself with F10.");

        public static string Install => T("Установить", "Install");
        public static string Reinstall => T("Переустановить", "Reinstall");
        public static string Update => T("Обновить", "Update");
        public static string Uninstall => T("Удалить мод", "Uninstall");
        public static string Browse => T("Выбрать папку игры...", "Choose game folder...");

        public static string ModeSaved(bool autoMode) => T(
            "Режим сохранён: " + (autoMode ? "автоматический" : "ручной (F10)") + ". Если игра запущена, он включится при следующем запуске.",
            "Mode saved: " + (autoMode ? "auto" : "manual (F10)") + ". If the game is running, it applies on the next launch.");

        public static string BrowseDescription => T(
            "Выбери папку, куда установлена Split Fiction",
            "Choose the folder where Split Fiction is installed");

        public static string ExeNotFound => T(
            "В этой папке не нашлось SplitFiction.exe. Выбери папку \"Split Fiction\" из steamapps\\common.",
            "SplitFiction.exe was not found in this folder. Choose the \"Split Fiction\" folder inside steamapps\\common.");

        public static string GameNotFound => T("Игра не найдена автоматически.", "The game was not found automatically.");
        public static string PressBrowse => T("Нажми \"Выбрать папку игры...\"", "Click \"Choose game folder...\"");
        public static string GameFolder(string path) => T("Папка игры: ", "Game folder: ") + path;
        public static string Outdated => T("Установлена старая версия мода. Нажми \"Обновить\".", "An older version of the mod is installed. Click \"Update\".");
        public static string Installed(string version) => T("Мод установлен (версия " + version + ")", "Mod installed (version " + version + ")");
        public static string NotInstalled => T("Мод не установлен", "Mod not installed");

        public static string CloseGameFirst => T(
            "Сначала закрой Split Fiction, потом попробуй ещё раз.",
            "Close Split Fiction first, then try again.");

        public static string InstallDone(bool autoMode) => T(
            "Мод установлен!\n\nЗапускай игру из Steam как обычно.\nВ игре жми F10: раздельный экран -> твой персонаж -> персонаж напарника."
                + (autoMode ? "\n\nАвтоматический режим включён: после катсцен и загрузок мод сам включает твой экран." : ""),
            "The mod is installed!\n\nLaunch the game from Steam as usual.\nIn game press F10: split screen -> your character -> your partner's character."
                + (autoMode ? "\n\nAuto mode is on: after cutscenes and loading screens the mod switches to your view by itself." : ""));

        public static string UninstallDone => T("Мод удалён, игра снова в исходном виде.", "The mod is removed, the game is back to its original state.");
        public static string Failed(string message) => T("Что-то пошло не так:\n\n", "Something went wrong:\n\n") + message;
        public static string NoAccessEvenAsAdmin(string path) => T("Нет доступа к папке игры даже с правами администратора:\n", "No access to the game folder even with administrator rights:\n") + path;

        public static string AskElevation => T(
            "Чтобы записать файлы в папку игры, нужны права администратора. Продолжить?",
            "Administrator rights are needed to write files into the game folder. Continue?");
    }
}
