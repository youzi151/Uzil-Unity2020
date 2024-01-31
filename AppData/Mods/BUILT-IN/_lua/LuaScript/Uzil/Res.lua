local this = {};


--[[ 取得文字 ]]
function this.getString (path, isForceReload)
	return UZAPI.Res.GetString(path, isForceReload);
end


return this;
