local MOD = "[FullFiction] "

local MODE_SPLIT = 1
local MODE_MINE = 2
local MODE_PARTNER = 3
local MODE_NAMES = { "split screen (game default)", "my view fullscreen", "partner view fullscreen" }

local SPLIT_MODE_CUSTOM_MERGE = 2
local SPLIT_MODE_MANUAL_VIEWS = 3

-- Override value that gives Mio the whole screen.
local MIO_FULLSCREEN_SHARE = 1.0
local AUTO_DELAY_TICKS = 10

local config = { AutoMode = false }
local ok, loaded = pcall(require, "fullfiction_config")
if ok and type(loaded) == "table" then config = loaded end

local mode = MODE_SPLIT
local overrideActive = false
local autoPending = true
local calmTicks = 0
local wasInCutscene = false
local lastState = nil

local gameStatics = nil
local sceneViewStatics = nil
local viewport = nil

local function log(msg)
    print(MOD .. msg .. "\n")
end

local function isValid(obj)
    return obj ~= nil and obj:IsValid()
end

local function setState(state)
    if state == lastState then return end
    lastState = state
    log(state)
end

local function getStatics()
    if not isValid(gameStatics) then
        gameStatics = StaticFindObject("/Script/Split.Default__HazeGameStatics")
    end
    if not isValid(sceneViewStatics) then
        sceneViewStatics = StaticFindObject("/Script/Split.Default__HazeSceneViewStatics")
    end
    return isValid(gameStatics) and isValid(sceneViewStatics)
end

-- The viewport client lives for the whole session, so it is looked up once instead of scanning all objects every tick.
local function getWorld()
    if not isValid(viewport) then
        viewport = FindFirstOf("HazeViewportClient")
        if not isValid(viewport) then return nil end
    end
    local world = viewport.World
    if not isValid(world) then return nil end
    return world
end

local function isInCutscene(player)
    return isValid(player) and (player.bIsControlledByCutscene or player.bIsParticipatingInCutscene)
end

local function clearOverride(world)
    if not overrideActive then return end
    overrideActive = false
    if world ~= nil then sceneViewStatics:ClearViewSizeOverride(world) end
end

local function pause(world, state)
    clearOverride(world)
    calmTicks = 0
    setState(state)
end

local function tick()
    if not getStatics() then return setState("waiting for game classes") end

    local world = getWorld()
    if world == nil then
        overrideActive = false
        return setState("waiting for world")
    end

    -- Leaving gameplay (loading, menu): drop the override before the next level starts rendering.
    local me = gameStatics:GetFirstLocalPlayer(world)
    if not isValid(me) then
        mode = MODE_SPLIT
        autoPending = true
        return pause(world, "no local player (menu or loading)")
    end

    if isInCutscene(gameStatics:GetMio(world)) or isInCutscene(gameStatics:GetZoe(world)) then
        if not wasInCutscene then log("cutscene started, handing the screen back to the game") end
        wasInCutscene = true
        mode = MODE_SPLIT
        autoPending = true
        return pause(world, "cutscene")
    end
    if wasInCutscene then
        wasInCutscene = false
        log("cutscene ended")
    end

    local splitMode = sceneViewStatics:GetSplitScreenMode(world)
    if splitMode == SPLIT_MODE_CUSTOM_MERGE or splitMode == SPLIT_MODE_MANUAL_VIEWS then
        return pause(world, "game uses a special screen layout, waiting")
    end

    if not overrideActive and sceneViewStatics:IsFullScreen(world) then
        return pause(world, "game shows its own fullscreen, waiting")
    end

    if config.AutoMode and autoPending then
        calmTicks = calmTicks + 1
        if calmTicks < AUTO_DELAY_TICKS then return end
        autoPending = false
        mode = MODE_MINE
        log("auto mode -> my view fullscreen")
    end

    if mode == MODE_SPLIT then
        clearOverride(world)
        return setState(MODE_NAMES[mode])
    end

    local showMio = me:IsMio()
    if mode == MODE_PARTNER then showMio = not showMio end
    local share = showMio and MIO_FULLSCREEN_SHARE or (1.0 - MIO_FULLSCREEN_SHARE)

    sceneViewStatics:SetViewSizeOverride(world, share)
    overrideActive = true
    setState(string.format("%s (%s, override=%.1f)", MODE_NAMES[mode], showMio and "Mio" or "Zoe", share))
end

local function safeTick()
    local success, err = pcall(tick)
    if not success then setState("error: " .. tostring(err)) end
end

LoopAsync(100, function()
    ExecuteInGameThread(safeTick)
    return false
end)

RegisterKeyBind(Key.F10, function()
    if wasInCutscene then
        log("F10 ignored during cutscene")
        return
    end
    autoPending = false
    mode = mode % #MODE_NAMES + 1
    log("F10 -> " .. MODE_NAMES[mode])
end)

log("loaded, auto mode " .. (config.AutoMode and "ON" or "OFF") .. ". F10 cycles: split -> my view -> partner view")
