--[[

	說明：
		on, once, any 傳入的 callback 為 function, 而非 Callback.new 建立出來的ID.
		因為這邊會把callback function 包入 自動建立的callback, 
		讓使用者不需要自行註銷兩種ID(EventBus.off與Callback.remove)
		只需要使用EventBus.off, EventBus就會自動把對應的Callback給移除掉

]]--

local this = {};

this.anonymousListenerID_prefix = "lua_eventBusListener_";
this.uniqID = UniqID.inst("eventBusListenerID");

this.keyAndID2CBKey = {}

--[[ 取得實例 ]] -- table
function this.inst (key)
	if (key == nil) then key = "_default" end

	local instance = {}

	instance.key = key

	-- [[ 註冊單一事件 ]] -- string
	instance.once = function(id, tag, callback)
		return this.once(instance.key, id, tag, callback)
	end

	--[[ 註冊事件 ]] -- string
	instance.on = function(id, tag, callback)
		return this.on(instance.key, id, tag, callback)
	end

	--[[ 註冊任意事件 ]] -- string
	instance.any = function(id, tag, callback) 
		return this.on(instance.key, id, tag, callback)
	end

	--[[ 排序事件 ]]
	instance.sort = function(id, tag, sort)
		this.sort(instance.key, id, tag, sort);
	end

	--[[ 排序任意事件 ]]
	instance.sortAny = function(id, sort)
		this.sortAny(instance.key, id, sort);
	end

	--[[ 註銷事件 ]]
	instance.off = function(id)
		this.off(instance.key, id)
	end

	--[[ 發送 ]]
	instance.post = function(tag, dataTable)
		this.post(instance.key, tag, dataTable)
	end

	--[[ 清除 ]]
	instance.clear = function ()
		this.clear(instance.key);
	end

	return instance
end

--[[ 預設提供 ]]

-- 主要
this.main = this.inst();


-- 註冊 ------------------

--[[ 註冊單一事件 ]] -- string
function this.once (eventBusKey, id, tag, callback)

	if (id == nil) then
		id = this.anonymousListenerID_prefix..tostring(this.uniqID.request())
	end

	local packedCallback = function(data)
		this.off(eventBusKey, id)
		callback(data)
	end

	-- 註冊
	local cbid = this.on(eventBusKey, id, tag, packedCallback)

	return cbid;
end

--[[ 註冊事件 ]] -- string
function this.on (eventBusKey, id, tag, callback)

	if (id == nil) then
		id = this.anonymousListenerID_prefix..tostring(this.uniqID.request());
	end
	local cbKey = "_EventBus_"..id;
	local keyAndID = eventBusKey..id;

	-- 將Callback包裝 並 取得CallbackID
	local cbFunc = (function(dataJson)
		if (dataJson ~= nil) then 
			callback(Json.decode(dataJson));
		else
			callback();
		end
	end);
	local cbID = Callback.getID(cbKey, cbFunc);

	-- 紀錄 EventListenerID 與 對應的CallbackKey
	this.keyAndID2CBKey[keyAndID] = cbKey;

	UZAPI.EventBus.On(tostring(eventBusKey), tostring(id), tostring(tag), cbID);

	return cbID;
end

--[[ 註冊任意事件 ]] -- string
function this.any (eventBusKey, id, callback)

	if (id == nil) then
		id = this.anonymousListenerID_prefix..tostring(this.uniqID.request())
	end

	local cbKey = "_EventBus_"..id
	local keyAndID = eventBusKey..id

	-- 將Callback包裝 並 取得CallbackID
	local cbFunc = (function(dataJson)
		if (dataJson ~= nil) then 
			callback(Json.decode(dataJson))
		else
			callback()
		end
	end)
	local cbID = Callback.getID(cbKey, cbFunc)

	-- 紀錄 EventListenerID 與 對應的CallbackKey
	this.keyAndID2CBKey[keyAndID] = cbKey

	UZAPI.EventBus.OnAny(tostring(eventBusKey), tostring(id), cbID)

	return id;
end

--[[ 排序事件 ]]
function this.sort (eventBusKey, id, tag, sort)
	UZAPI.EventBus.Sort(tostring(eventBusKey), tostring(id), tag, sort);
end

--[[ 排序事件 ]]
function this.sortAny (eventBusKey, id, sort)
	UZAPI.EventBus.SortAny(tostring(eventBusKey), tostring(id), sort);
end

--[[ 註銷事件或任意事件 ]]
function this.off (eventBusKey, id)
	if (id == nil) then return end

	UZAPI.EventBus.Off(eventBusKey, id);
	local keyAndID = eventBusKey..id

	local cbKey = this.keyAndID2CBKey[keyAndID]
	if (cbKey == nil) then return end

	-- 註銷Callback (之前自動生成的)
	Callback.removeKey(cbKey)

	-- 檢查 若ID 為 自動派發 則 釋放該ID
	local id_length = string.len(id);
	local prefix_length = string.len(this.anonymousListenerID_prefix);

	if (id_length > prefix_length and Util.startWith(id, this.anonymousListenerID_prefix)) then
		local toRmID = string.sub(id, prefix_length+1, id_length);
		toRmID = tonumber(toRmID)
		if (toRmID ~= nil) then
			this.uniqID.release(toRmID);
		end
	end

end

--[[ 發送 ]]
function this.post (eventBusKey, tag, dataTable)
	local data_str;
	if (type(dataTable) == "string") then
		data_str = dataTable;
	else
		data_str = Json.encode(dataTable);
	end
	UZAPI.EventBus.Post(eventBusKey, tag, data_str);
end

--[[ 清空 ]]
function this.clear (eventBusKey)
	UZAPI.EventBus.Clear(eventBusKey);
end

	
return this;