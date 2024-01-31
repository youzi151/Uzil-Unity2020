local this = {};


--[[ 發射特效 ]] -- string
function this.emit (sourcePath, initData, onDoneCB)
	local initData_str = Json.encode(initData);

	local cbID = -1;
	if (onDoneCB ~= nil) then
		cbID = Callback.newID(nil, onDoneCB);
	end

	local id;
	if (cbID ~= -1) then
		id = UZAPI.FX.Emit(nil, sourcePath, initData_str, cbID);
	else
		id = UZAPI.FX.Emit(nil, sourcePath, initData_str);
	end
	return id;
end

--[[ 預載 ]] -- string
function this.preload (preferID, sourcePath, initData)
	local initData_str = Json.encode(initData);
	return UZAPI.FX.Preload(preferID, sourcePath, initData_str);
end

--[[ 設置資料 ]]
function this.setData (id, data)
	local data_str = Json.encode(data);
	UZAPI.FX.SetData(id, data_str);
end

--[[ 播放特效 ]] -- void
function this.play (id, playData)
	local playData_str = Json.encode(playData);
	UZAPI.FX.Play(id, playData_str);
end

--[[ 停止特效 ]] -- void
function this.stop (id)
	UZAPI.FX.Stop(id);
end

--[[ 銷毀特效 ]] -- void
function this.destroy (id)
	UZAPI.FX.Destroy(id);
end

--[[ 呼叫 ]] -- void
function this.call (id, msg, callData)
	local callData_str = Json.encode(callData);
	UZAPI.FX.Call(id, msg, callData_str);
end


return this;