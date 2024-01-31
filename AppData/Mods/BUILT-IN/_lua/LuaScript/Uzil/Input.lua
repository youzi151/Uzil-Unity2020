local this = {};

this.InputType = {
	["none"] = 0,
	["key"] = 1,
	["axis"] = 2
};

this.KeyState = {
	["none"] = 0,
	["down"] = 1,
	["stay"] = 2,
	["up"] = 3
};

local keyCode = require("Uzil/KeyCode");
this.KeyCode = keyCode.enum;

this.AllKeyCodes = keyCode.keyCodes;

function this.inst (key)

	local inst = {};
	inst.key = key;

	--[[ 取得 輸入 ]]
	inst.getInput = function (layerID, virtualKeyCode)
		return this.getInput(inst.key, layerID, virtualKeyCode);
	end

	--[[ 取得 輸入 值 數值 ]]
	inst.getInputValueNum = function (layerID, virtualKeyCode)
		return this.getInputValueNum(inst.key, layerID, virtualKeyCode);
	end

	--[[ 取得 輸入 值 字串 ]]
	inst.getInputValueStr = function (layerID, virtualKeyCode)
		return this.getInputValueStr(inst.key, layerID, virtualKeyCode);
	end

	--[[ 按鍵按下 ]]
	inst.getKeyDown = function (layerID, virtualKeyCode)
		return this.getKeyDown(inst.key, layerID, virtualKeyCode);
	end

	--[[ 按鍵按著 ]]
	inst.getKey = function (layerID, virtualKeyCode)
		return this.getKey(inst.key, layerID, virtualKeyCode);
	end

	--[[ 按鍵彈起 ]]
	inst.getKeyUp = function (layerID, virtualKeyCode)
		return this.getKeyUp(inst.key, layerID, virtualKeyCode);
	end

	--[[ 註冊 當輸入 ]]
	inst.addOnInput = function (layerID, virtualKeyCode, id, cb, sort)
		this.addOnInput(inst.key, layerID, virtualKeyCode, id, cb, sort);
	end

	--[[ 註銷 當輸入 ]]
	inst.removeOnInput = function (layerID, virtualKeyCode, id)
		this.removeOnInput(inst.key, layerID, virtualKeyCode, id);
	end

	--[[ 新增 處理器 ]]
	inst.addHandler = function (layerID, id, data)
		this.addHandler(inst.key, layerID, id, data);
	end

	--[[ 移除 處理器 ]]
	inst.removeHandler = function (layerID, id)
		this.removeHandler(inst.key, layerID, id);
	end

	return inst;
end

--[[ 取得 輸入 ]]
function this.getInput (instID, layerID, virtualKeyCode)
	return UZAPI.Input.GetInput(instID, layerID, virtualKeyCode);
end

--[[ 取得 輸入 值 數值 ]]
function this.getInputValueNum (instID, layerID, virtualKeyCode)
	return UZAPI.Input.GetInputValueNum(instID, layerID, virtualKeyCode);
end

--[[ 取得 輸入 值 字串 ]]
function this.getInputValueStr (instID, layerID, virtualKeyCode)
	return UZAPI.Input.GetInputValueStr(instID, layerID, virtualKeyCode);
end

--[[ 按鍵按下 ]]
function this.getKeyDown (instID, layerID, virtualKeyCode)
	return UZAPI.Input.GetKeyDown(instID, layerID, virtualKeyCode);
end

--[[ 按鍵按著 ]]
function this.getKey (instID, layerID, virtualKeyCode)
	return UZAPI.Input.GetKey(instID, layerID, virtualKeyCode);
end

--[[ 按鍵彈起 ]]
function this.getKeyUp (instID, layerID, virtualKeyCode)
	return UZAPI.Input.GetKeyUp(instID, layerID, virtualKeyCode);
end

--[[ 註冊 當輸入 ]]
function this.addOnInput (instID, layerID, virtualKeyCode, id, cb, sort)
	local key = "input."..instID.."."..layerID..tostring(virtualKeyCode)..id;
	local listenerID = key;
	local cbid = Callback.getID(key, function (inputSignal_str)
		local inputSignal = Json.decode(inputSignal_str);
		-- if (inputSignal.isAlive == false) then return end
		cb(inputSignal);
	end, "input");
	if (sort == nil) then sort = 0 end
	UZAPI.Input.AddOnInput(instID, layerID, virtualKeyCode, listenerID, cbid, sort);
end

--[[ 註銷 當輸入 ]]
function this.removeOnInput (instID, layerID, virtualKeyCode, id)
	local listenerID = "input."..instID.."."..layerID..tostring(virtualKeyCode)..id;
	UZAPI.Input.RemoveOnInput(instID, layerID, virtualKeyCode, listenerID);
end

--[[ 新增 處理器 ]]
function this.addHandler (instID, layerID, id, data)
	UZAPI.Input.AddHandler(instID, layerID, id, Json.encode(data));
end

--[[ 移除 處理器 ]]
function this.removeHandler (instID, layerID, id)
	UZAPI.Input.RemoveHandler(instID, layerID, id);
end

return this;