local this = {}

-- 條件庫
this.gateTable = {}


-- 匯入條件  (for lua)----------------------------
function this.import(gateName, gate)
	this.gateTable[gateName] = gate
	-- print("import", gateName)
end


-- 檢查是否存在  ----------------------------------
function this.isExist(gateName)
	return (this.gateTable[gateName] ~= nil)
end


-- 各個時機   (for C#)----------------------------

--條件開始
function this.callBegin(gateName, pMgrKey, gateID)
	if (this.gateTable[gateName] == nil) then
		return nil
	else
		-- 取得條件
		local gate = this.gateTable[gateName]
		gate:begin(pMgrKey, gateID)
	end
end

--條件暫停
function this.callPause(gateName, pMgrKey, gateID)
	if (this.gateTable[gateName] == nil) then
		return nil
	else
		-- 取得條件
		local gate = this.gateTable[gateName]
		gate:pause(pMgrKey, gateID)
	end
end

--條件繼續
function this.callResume(gateName, pMgrKey, gateID)
	if (this.gateTable[gateName] == nil) then
		return nil
	else
		-- 取得條件
		local gate = this.gateTable[gateName]
		gate:resume(pMgrKey, gateID)
	end
end

--條件結束
function this.callEnd(gateName, pMgrKey, gateID)
	if (this.gateTable[gateName] == nil) then
		return nil
	else
		-- 取得條件
		local gate = this.gateTable[gateName]
		gate:finish(pMgrKey, gateID)
	end
end

--條件重置
function this.callReset(gateName, pMgrKey, gateID)
	if (this.gateTable[gateName] == nil) then
		return nil
	else
		-- 取得條件
		local gate = this.gateTable[gateName]
		gate:reset(pMgrKey, gateID)
	end
end


-- 觸發事件 ---------------------------------------


return this;