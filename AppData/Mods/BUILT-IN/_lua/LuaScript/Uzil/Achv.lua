local this = {};

--[[ 是否 已解鎖 ]]
function this.isUnlock (id)
	return UZAPI.Achv.IsUnlock(id);
end

--[[ 設置 進度 ]]
function this.getStat (id)
	return UZAPI.Achv.GetStat(id);
end

--[[ 設置 進度 ]]
function this.setStat (id, progress_int)
	UZAPI.Achv.SetStat(id, progress_int);
end

--[[ 設置 完成 ]]
function this.setDone (id)
	UZAPI.Achv.SetDone(id);
end

return this;