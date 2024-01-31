local this = {};

--[[ 取得實例 ]] -- table
function this.inst (pMgrKey)

	local pMgr = {};

	pMgr.key = pMgrKey;

	--[[ 節點開始 ]] -- void
	pMgr.nodeBegin = function (nodeID)
		this.nodeBegin (pMgr.key, nodeID);
	end

	--[[ 節點開始 ]] -- void
	pMgr.nodeEnd = function (nodeID)
		this.nodeEnd (pMgr.key, nodeID);
	end

	--[[ 創建/設置節點 ]] -- string
	pMgr.nodeSet = function (nodeID, dataOrName)
		return this.nodeSet(pMgr.key, nodeID, dataOrName);
	end
	--[[ 移除節點 ]] -- void
	pMgr.nodeDel = function (nodeID)
		this.nodeDel(pMgr.key, nodeID);
	end

	--[[ 創建/設置事件 ]] -- string
	pMgr.eventSet = function (eventID, dataOrName)
		return this.eventSet(pMgr.key, eventID, dataOrName);
	end
	--[[ 移除事件 ]] -- void
	pMgr.eventDel = function (eventID)
		this.eventDel(pMgr.key, eventID);
	end

	--[[ 創建/設置條件 ]] -- string
	pMgr.gateSet = function (gateID, dataOrName)
		return this.gateSet(pMgr.key, gateID, dataOrName);
	end
	--[[ 移除條件 ]] -- void
	pMgr.gateDel = function (gateID)
		this.gateDel(pMgr.key, gateID);
	end


	--[[ 條件開始 ]] -- void
	pMgr.gateComplete = function (conditionID)
		this.gateComplete (pMgr.key, conditionID);
	end

	--[[ 條件檢查 ]] -- void
	pMgr.gateCheck = function (conditionID)
		this.gateCheck (pMgr.key, conditionID);
	end

	--[[ 條件開始 ]] -- void
	pMgr.gateBegin = function (conditionID)
		this.gateBegin (pMgr.key, conditionID);
	end

	--[[ 條件暫停 ]] -- void
	pMgr.gatePause = function (conditionID)
		this.gatePause (pMgr.key, conditionID);
	end

	--[[ 條件繼續 ]] -- void
	pMgr.gateResume = function (conditionID)
		this.gateResume (pMgr.key, conditionID);
	end

	--[[ 條件結束 ]] -- void
	pMgr.gateEnd = function (conditionID)
		this.gateEnd (pMgr.key, conditionID);
	end

	--[[ 條件重置 ]] -- void
	pMgr.gateReset = function (conditionID)
		this.gateReset (pMgr.key, conditionID);
	end

	--[[ 條件設置參數 ]] -- void
	pMgr.gateSetArgs = function (conditionID, args)
		this.gateSetArgs (pMgr.key, conditionID, args);
	end

	pMgr.debug_log = function ()
		return this.debug_log(pMgr.key);
	end

	
	--[[ 快速 建立/設置 Lua條件 ]] -- string
	pMgr.luaGate = function (onEventTags, isComplete_lua_bool)
		return this.luaGate(pMgr.key, onEventTags, isComplete_lua_bool);
	end

	--[[ 快速 建立/設置 Lua節點、事件 ]] -- table
	pMgr.luaNodeEvent = function (lua_str)
		return this.luaNodeEvent(pMgr.key, lua_str);
	end

	return pMgr;
end


--[[ 節點開始 ]] -- void
function this.nodeBegin (pMgrKey, nodeID)
	UZAPI.Proc.NodeBegin(pMgrKey, nodeID);
end

--[[ 節點開始 ]] -- void
function this.nodeEnd (pMgrKey, nodeID)
	UZAPI.Proc.NodeEnd(pMgrKey, nodeID);
end

--[[ 創建/設置節點 ]] -- string
function this.nodeSet (pMgrKey, nodeID, dataOrName)
	local data_str;
	if (type(dataOrName) == "table") then
		data_str = Json.encode(dataOrName);
	else
		data_str = dataOrName;
	end

	local id = UZAPI.Proc.NodeSet(pMgrKey, nodeID, data_str);
	return id;
end
--[[ 移除節點 ]] -- void
function this.nodeDel (pMgrKey, nodeID)
	UZAPI.Proc.NodeDel(pMgrKey, nodeID);
end

--[[ 創建/設置事件 ]] -- string
function this.eventSet (pMgrKey, eventID, dataOrName)
	local data_str;
	if (type(dataOrName) == "table") then
		data_str = Json.encode(dataOrName);
	else
		data_str = dataOrName;
	end

	local id = UZAPI.Proc.EventSet(pMgrKey, eventID, data_str);
	return id;
end
--[[ 移除事件 ]] -- void
function this.eventDel (pMgrKey, eventID)
	UZAPI.Proc.EventDel(pMgrKey, eventID);
end

--[[ 創建/設置條件 ]] -- string
function this.gateSet (pMgrKey, gateID, dataOrName)
	local data_str;
	if (type(dataOrName) == "table") then
		data_str = Json.encode(dataOrName);
	else
		data_str = dataOrName;
	end

	local id = UZAPI.Proc.GateSet(pMgrKey, gateID, data_str);
	return id;
end
--[[ 移除條件 ]] -- void
function this.gateDel (pMgrKey, gateID)
	UZAPI.Proc.GateDel(pMgrKey, gateID);
end

--[[ 條件開始 ]] -- void
function this.gateComplete (pMgrKey, conditionID)
	UZAPI.Proc.GateComplete(pMgrKey, conditionID);
end

--[[ 條件檢查 ]] -- void
function this.gateCheck (pMgrKey, conditionID)
	UZAPI.Proc.GateCheck(pMgrKey, conditionID);
end

--[[ 條件開始 ]] -- void
function this.gateBegin (pMgrKey, conditionID)
	UZAPI.Proc.GateBegin(pMgrKey, conditionID);
end

--[[ 條件暫停 ]] -- void
function this.gatePause (pMgrKey, conditionID)
	UZAPI.Proc.GatePause(pMgrKey, conditionID);
end

--[[ 條件繼續 ]] -- void
function this.gateResume (pMgrKey, conditionID)
	UZAPI.Proc.GateResume(pMgrKey, conditionID);
end

--[[ 條件結束 ]] -- void
function this.gateEnd (pMgrKey, conditionID)
	UZAPI.Proc.GateEnd(pMgrKey, conditionID);
end

--[[ 條件重置 ]] -- void
function this.gateReset (pMgrKey, conditionID)
	UZAPI.Proc.GateReset(pMgrKey, conditionID);
end

--[[ 條件設置參數 ]] -- void
function this.gateSetArgs (pMgrKey, conditionID, args)
	local args_str = Json.encode(args);
	UZAPI.Proc.GateSetArgs(pMgrKey, conditionID, args_str);
end

--[[ 除錯用 ]]
function this.debug_log (pMgrKey)
	return UZAPI.Proc.Debug_Log(pMgrKey);
end

--[[ 快速 建立/設置 Lua條件 ]] -- string
function this.luaGate (pMgrKey, onEventTags, isComplete_luaStr_bool)

	local gateJson = Json.encode({
		["gate"] = "EventBus",
		["param"] = {
			["eventTags"] = onEventTags,
			["luaScript"] = isComplete_luaStr_bool;
		}
	});
	local gateID = Proc.gateSet(pMgrKey, nil, gateJson);

	return gateID;
end

--[[ 快速 建立/設置 Lua節點、條件 ]] -- string
function this.luaNodeGate (pMgrKey, onEventTags, isComplete_luaStr_bool)
	
	local gateID = this.luaGate(pMgrKey, onEventTags, isComplete_luaStr_bool);

	local nodeJson = Json.encode({
		["node"] = "General",
		["param"] = {
			["gateList"] = {gateID};
		}
	});
	local nodeID = Proc.nodeSet(pMgrKey, nil, nodeJson);

	return nodeID, gateID;
end

--[[ 快速 建立/設置 Lua節點、事件 ]] -- table
function this.luaNodeEvent (pMgrKey, lua_str)

	local eventJson = Json.encode({
		["event"] = "Lua",
		["param"] = {
			["luaScript"] = lua_str;
		}
	});
	local eventID = Proc.eventSet(pMgrKey, nil, eventJson);

	local nodeJson = Json.encode({
		["node"] = "General",
		["param"] = {
			["eventList"] = {eventID};
		}
	});
	local nodeID = Proc.nodeSet(pMgrKey, nil, nodeJson);

	return nodeID, eventID;

end




return this;