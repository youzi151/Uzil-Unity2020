-- 此檔案為遊戲建立Lua實體時，自動執行之主要入口

(function ()

	-- 全域
		
	Global = {};

	-- require --------

	-- 暫存require的結果, 由C#方設置
	Global.RequireResTemp = nil;

	-- 已註冊 的
	local loaded = {};

	-- 包裝require
	local _require = require;
	requireInMod = function (path)
		-- 試取得 已註冊
		local res = loaded[path];
		if (res ~= nil) then return res end

		-- 呼叫執行 require (會將結果設置於Global.RequireResTemp)
		UZAPI.Util.LuaRequire(path);

		-- 取出結果 並 消除暫存
		res = Global.RequireResTemp;
		Global.RequireResTemp = nil;
		-- 若 沒取得 則 使用 原require
		if (res == nil) then res = _require(path) end
		-- 若 沒取得 則 返回 空
		if (res == nil) then return nil end

		-- 註冊
		loaded[path] = res;

		return res;
	end
	require = requireInMod;

	-- Uzil ----------

	-- 必備 (系統層級 提供給 Lua/C# 層溝通使用)
	Json = require "Uzil/Json";
	Math = require "Uzil/Math";
	Util = require "Uzil/Util";
	Flags = require "Uzil/Flags";
	Res = require "Uzil/Res";
	Vec2 = require "Uzil/Vector2";
	Async = require "Uzil/Async";
	DoPass = require "Uzil/DoPass";
	Cache = require "Uzil/Cache";
	Var = require "Uzil/Var";
	Vals = require "Uzil/Vals";
	UniqID = require "Uzil/UniqID";
	Invoker = require "Uzil/Invoker";
	I18n = require "Uzil/I18n";
	Random = require "Uzil/Random";
	Callback = require "Uzil/Callback";
	EventBus = require "Uzil/EventBus";
	Proc = require "Uzil/Proc";
	Config = require "Uzil/Config";
	UserSave = require "Uzil/UserSave";
	ProfileSave = require "Uzil/ProfileSave";

	-- 其他功能
	Audio = require "Uzil/Audio";
	FX = require "Uzil/FX";
	UI = require "Uzil/UI";
	Camera = require "Uzil/Camera";
	Input = require "Uzil/Input";
	PostProc = require "Uzil/PostProc";
	BlockPage = require "Uzil/BlockPage";
	Option = require "Uzil/Option";

	-----------------

	-- Custom -------

	-- xxx = require "XXX/XXX";

	-----------------

	-- 腳本庫
	ScriptDB = require "ScriptDB";

end)();