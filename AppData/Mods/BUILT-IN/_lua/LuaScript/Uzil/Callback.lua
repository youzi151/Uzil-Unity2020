local this = {};

this.uniqID_dispatchIndex = UniqID.inst("_callback");
this.debugIdx = UniqID.inst("_callback_debug");

--[[ key對應id ]]
this.key2id = {}

--[[ id對應callback{func, key, debugMsg} ]]
this.pool = {}

--[[ 是否存在 ]]
function this.isExist (key)
	local id = this.key2id[key];
	return id ~= nil;
end

--[[ 請求一個新的Callback ID ]]
-- 參數(必：func  選：key)
-- 若有指定key 則 callback會保持存活，直到呼叫remove
-- 若指定key已經有對應的callback，則會移除該callback的key
function this.newID (key, func, debugMsg)

	local id = this.uniqID_dispatchIndex.request();

	local cb = {};

	cb.func = func;

	-- 若有指定key
	if (key ~= nil) then

		-- 指定key 目前對應的 Callback
		local lastID = this.key2id[key];
		-- 若 存在 則 清空該Callback的key
		if (lastID ~= nil) then
			local lastCB = this.pool[lastID];
			lastCB.key = nil;
		end

		-- 設置
		cb.key = key;
		this.key2id[key] = id;
	end

	if (debugMsg ~= nil) then
		cb.debugMsg = debugMsg;
	end

	this.pool[id] = cb;

	-- local logMsg = "reg Callback key:"..tostring(key).." / cbid:"..tostring(id);
	-- if (debugMsg ~= nil) then
	-- 	logMsg = logMsg.." debugMsg:"..debugMsg;
	-- end
	-- print(logMsg);

	return id;
end

--[[ 取得Callback的ID，若不存在則建立 ]]
-- 參數(必：key  選：func)，若有參數func則覆蓋
function this.getID (key, func, debugMsg)
	local id = this.key2id[key];

	-- 若已有存在
	if (id ~= nil) then

		-- 覆蓋func
		if (func ~= nil) then
			this.pool[id].func = func;
		end

		this.pool[id].debugMsg = debugMsg;

		return id;
	end

	if (func == nil) then return nil end

	return this.newID(key, func, debugMsg);
end

--[[ 以ID呼叫Callback (最高支援 4 個額外參數) ]]
function this.call (id, arg1, arg2, arg3, arg4)
	if (id == -1 or id == nil) then return end

	-- local debug = this.debugIdx.request();
	-- print("["..debug.."] call: "..id);

	local callback = this.pool[id];
	if (callback == nil) then
		print("[Callback.lua] callback["..tostring(id).."] not exist.");
		return
	end

	-- print("id["..id.."] key["..tostring(callback.key).."] ".."msg["..tostring(callback.debugMsg).."]");

	-- 若沒有指定key 則 視為匿名 並 移除
	local key = callback.key;
	if (key == nil) then
		this.removeID(id);
	end

	local res;

	-- 執行Callback
	-- 放在最後是因為func中有可能對callback進行其他操作
	if (arg1 ~= nil) then
		res = callback.func(arg1, arg2, arg3, arg4);
	else
		res = callback.func();
	end

	-- print("["..debug.."] call end : "..id);

	return res;
end

--[[ 移除callback ]]
function this.removeID (id)

	local cb = this.pool[id];
	if (cb == nil) then return end

	-- 移除CB
	this.pool[id] = nil;
	this.uniqID_dispatchIndex.release(id);

	-- 移除key
	if (cb.key ~= nil) then
		this.key2id[cb.key] = nil;
	end

	-- print("removeID: "..id.." [key]"..tostring(cb.key));
end

--[[ 移除callback ]]
function this.removeKey (key)
	local id = this.key2id[key];
	if (id == nil) then return end

	-- 移除key
	this.key2id[key] = nil;

	-- 移除CB
	this.pool[id] = nil;
	this.uniqID_dispatchIndex.release(id);

	-- print("removeKey: "..key.."[id]"..tostring(id));
end


return this;