local this = {};

--[[ 取得實例 ]] -- table
function this.inst(key)

	local instance = {};

	instance.key = key;

	-- [[ 呼叫 ]]
	instance.once = function (func, time, tagOrTags) 
		return this.once(instance.key, func, time, tagOrTags);
	end

	-- [[ 取消 ]]
	instance.cancel = function (tagOrTags) 
		this.cancel(instance.key, tagOrTags);
	end

	--[[ 清空 ]]
	instance.clear = function ()
		this.clear(instance.key);
	end

	-- [[ 呼叫 每幀 ]]
	instance.update = function (func, tagOrTags)
		this.update(instance.key, func, tagOrTags);
	end

	-- [[ 取消 每幀 ]]
	instance.updateCancel = function (tagOrTags) 
		this.updateCancel(instance.key, tagOrTags);
	end


	return instance;
end

--[[ 呼叫 ]]
function this.once (instID, func, time, tagOrTags)
	-- 確保 tags 為 複數
	local tags = Util.oneToArray(tagOrTags);
	-- 若時間未指定 則 預設0
	if (time == nil) then time = 0 end;
	-- 取得CBID
	local cbid = Callback.newID(nil, func, "invoker.once");
	-- 於 C#層 註冊 單次呼叫
	UZAPI.Invoker.Once(instID, cbid, time, Json.encode(tags));

	return cbid;
end

--[[ 取消 ]]
function this.cancel (instID, tagOrTags)
	local tags = Util.oneToArray(tagOrTags);
	UZAPI.Invoker.Cancel(instID, Json.encode(tags));
end

--[[ 清空 ]]
function this.clear (instID)
	UZAPI.Invoker.Clear(instID);
end

--[[ 呼叫 每幀 ]]
function this.update (instID, func, tagOrTags)
	local tags = Util.oneToArray(tagOrTags);

	local keyBase = "_invokerUpdate."..tostring(os.time());
	local key = keyBase;
	local fix = 1;

	while (Callback.isExist(key)) do
		key = keyBase.."."..tostring(fix);
		fix = fix + 1;
	end

	local cbid = Callback.getID(key, func);

	UZAPI.Invoker.Update(instID, cbid, Json.encode(tags));
end

--[[ 取消 每幀 ]]
function this.updateCancel (instID, tagOrTags)
	local tags = Util.oneToArray(tagOrTags);
	UZAPI.Invoker.UpdateCancel(instID, Json.encode(tags));
end

return this;