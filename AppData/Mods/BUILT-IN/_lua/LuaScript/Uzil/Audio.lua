local this = {};


--[[ 設置音量 ]]
function this.setVolumeLinear (mixerName, volume)
	UZAPI.Audio.SetVolumeLinear(mixerName, volume);
end

--[[ 取得音量 ]]
function this.getVolumeLinear (mixerName)
	return UZAPI.Audio.GetVolumeLinear(mixerName);
end

--[[ 是否存在 ]] -- bool
function this.isExist(id)
	return UZAPI.Audio.IsExist(id);
end

-- [[ 預載 ]] -- string
function this.preload (preferID, pathOrKey, args)
	return UZAPI.Audio.Preload(preferID, pathOrKey, Json.encode(args));
end

-- [[ 預載 ]] -- void
function this.preloadAsync (preferID, pathOrKey, args, onDone)
	local cbID;
	cbID = Callback.newID(nil, function (id)
		if (onDone ~= nil) then onDone(id) end
	end);
	return UZAPI.Audio.PreloadAsync(preferID, pathOrKey, Json.encode(args), cbID);
end

-- [[ 申請 ]] -- string
function this.request (preferID, pathOrKey, args)
	return UZAPI.Audio.Request(preferID, pathOrKey, Json.encode(args));
end

-- [[ 設置 ]] -- void
function this.set (id, args)
	UZAPI.Audio.Set(id, Json.encode(args));
end

-- [[ 播放 ]] -- void
function this.play (id)
	UZAPI.Audio.Play(id);
end

-- [[ 播放音效(方便使用) ]] -- string
function this.playSFX (pathOrKey, args)
	local args_str = Json.encode(args);
	local id = UZAPI.Audio.PlaySFX(pathOrKey, args_str);
	return id;
end

-- [[ 釋放 ]] -- void
function this.release (id)
	UZAPI.Audio.Release(id);
end

-- [[ 停止 ]] -- void
function this.stop (id)
	UZAPI.Audio.Stop(id);
end
-- [[ 停止所有 ]] -- void
function this.stopAll ()
	UZAPI.Audio.StopAll();
end

-- [[ 復原 ]] -- void
function this.resume (id)
	UZAPI.Audio.Resume(id);
end
-- [[ 復原所有 ]] -- void
function this.resumeAll ()
	UZAPI.Audio.ResumeAll();
end

-- [[ 暫停 ]] -- void
function this.pause (id)
	UZAPI.Audio.Pause(id);
end
-- [[ 暫停所有 ]] -- void
function this.pauseAll ()
	UZAPI.Audio.PauseAll();
end

return this;