local Cache = {};

--[[
	快取 類似Redis
	所設置與取得的對象，必須不會自行變動(e.g. 在C#中會自行運算改變或被其他C#程式變動)
	否則 get 只會從快取中取得而非實際的值。
]]


--[[ ID對應實體 ]]
Cache.id2Inst = {};

--[[ 取得實體 ]]
function Cache.inst (key)
	if (Util.containsKey(Cache.id2Inst, key)) then
		return Cache.id2Inst[key];
	end

	local inst = {};

	local dict = {};

	--[[ 是否存在 ]]
	inst.isExist = function (key)
		return Util.containsKey(dict, key);
	end

	--[[ 設置 到 快取中 ]]
	inst.set = function (key, value, saveFunc)
		Cache._set(dict, key, value, saveFunc);
	end

	--[[ 取得 從 快取中 ]]
	inst.get = function (key, loadFunc)
		return Cache._get(dict, key, loadFunc);
	end

	--[[ 移除 從 快取中 ]]
	inst.del = function (key, delFunc)
		Cache._del(dict, key, delFunc);
	end

	--[[ 清除 ]]
	inst.clear = function ()
		dict = {};
	end

	--[[ 取得所有 ]]
	inst.getAll = function (isDeepCopy)
		if (isDeepCopy) then
			return Util.deepCopy(dict);
		else 
			return dict;
		end
	end

	Cache.id2Inst[key] = inst;
	return inst;
end

--[[ 設置 到 快取中 ]]
-- 並 執行 寫入
function Cache._set (dict, key, value, saveFunc)
	dict[key] = value;
	if (saveFunc ~= nil) then saveFunc(key, value) end
end

--[[ 取得 從 快取中 ]]
-- 若 快取中不存在 則 執行讀取
function Cache._get (dict, key, loadFunc)
	if (Util.containsKey(dict, key)) then
		return dict[key];
	end

	if (loadFunc ~= nil) then
		local loaded = loadFunc(key);
		dict[key] = loaded;
		return loaded;
	end

	return nil;
end

--[[ 移除 從 快取中 ]]
-- 並 執行 移除
function Cache._del (dict, key, delFunc)
	if (Util.containsKey(dict, key)) then
		Util.remove(dict, key);
	end

	if (delFunc ~= nil) then delFunc(key) end
end


return Cache;