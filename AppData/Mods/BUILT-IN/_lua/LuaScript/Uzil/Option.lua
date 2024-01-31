local this = {};

-- 語言 -------------------------

--[[ 設置語言 ]]
function this.setLanguage (langCode)
	return UZAPI.Option.SetLanguage(langCode);
end

--[[ 取得語言 ]]
function this.getLanguage ()
	return UZAPI.Option.GetLanguage();
end

--[[ 取得語言列表 ]]
function this.getLanguageInfos ()
	return Json.decode(UZAPI.Option.GetLanguageInfos());
end

-- 音效 -------------------------

--[[ 設置音量 ]]
function this.setVolumeLinear (mixerName, volume)
    UZAPI.Option.SetVolumeLinear(mixerName, volume);
end

--[[ 取得音量 ]]
function this.getVolumeLinear (mixerName)
    return UZAPI.Option.GetVolumeLinear(mixerName);
end

-- 顯示 -------------------------

--[[ 取得 可支援的解析度 ]]
function this.getSupportResolutions ()
	return Json.decode(UZAPI.Option.GetSupportResolutions());
 end

--[[ 取得 解析度 ]]
function this.getResolution ()
	return Json.decode(UZAPI.Option.GetResolution());
end

--[[ 設置 解析度 ]]
function this.setResolution (width, height)
   UZAPI.Option.SetResolution(width, height);
end

--[[ 視窗模式 ]]
this.FullScreenMode = Util.enum({
	["fullScreen"] = 1,
	["window"] = 2,
});
--[[ 設置 視窗模式 ]]
function this.setFullScreenMode (fullScreenMode)
   UZAPI.Option.SetFullScreenMode(fullScreenMode);
end

--[[ 取得 視窗模式 ]]
function this.getFullScreenMode ()
	return UZAPI.Option.GetFullScreenMode();
 end


-- 操作 -------------------------

--[[ 取得 按鍵綁定 ]]
function this.getKeyBindings ()
	local id2SrcKeys_str = UZAPI.Option.GetKeyBindings();
	return Json.decode(id2SrcKeys_str);
end

--[[ 設置 按鍵綁定 ]]
function this.setKeyBinding (bindingID, srcKeys)
	UZAPI.Option.SetKeyBinding(bindingID, Json.encode(srcKeys));
end

-- 其他 -------------------------

--[[ 設置 是否於背景執行 ]]
function this.setRunInBackground (isRunInBG)
	UZAPI.Option.SetRunInBackground(isRunInBG);
end

--[[ 是否 於背景執行 ]]
function this.isRunInBackground ()
	return UZAPI.Option.IsRunInBackground();
end

--[[ 是否 可以於背景執行 ]]
function this.isRunInBackgroundAble ()
	return UZAPI.Option.IsRunInBackgroundAble();
end

return this;