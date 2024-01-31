local Config = {};

function Config.inst (path)

	local this = {};
	this.path = path;

	this.isExist = function (key)
		return Config.isExist(this.path, key);
	end

	this.setStr = function (key, val)
		return Config.setStr(this.path, key, val);
	end

	this.setNum = function (key, val)
		return Config.setNum(this.path, key, val);
	end

	this.setTable = function (key, val)
		return Config.setTable(this.path, key, val);
	end

	this.getStr = function (key)
		return Config.getStr(this.path, key);
	end

	this.getInt = function (key)
		return Config.getInt(this.path, key);
	end

	this.getFloat = function (key)
		return Config.getFloat(this.path, key);
	end

	this.getBool = function (key)
		return Config.getBool(this.path, key);
	end

	this.getTable = function (key)
		return Config.getTable(this.path, key);
	end

	return this;
end
Config.file = Config.inst;

--[[ 是否 存在 ]] -- bool
function Config.isExist (path, key)
   return tobool(UZAPI.Config.IsExist(path, key));
end
--[[ 設置 字串 ]] -- bool
function Config.setStr (path, key, val)
	return tobool(UZAPI.Config.SetStr(path, key, val));
end

--[[ 設置 數字 ]] -- bool
function Config.setNum (path, key, val)
	return tobool(UZAPI.Config.SetFloat(path, key, val));
end

--[[ 設置 物件 ]] -- bool
function Config.setTable (path, key, val)
	if (Util.isArray(val)) then
		return tobool(UZAPI.Config.SetList(path, key, Json.encode(val)));
	else
		return tobool(UZAPI.Config.SetObj(path, key, Json.encode(val)));
	end
end

--[[ 取得 字串 ]] -- string
function Config.getStr (path, key)
	return UZAPI.Config.GetStr(path, key);
end

--[[ 取得 變數 ]] -- int
function Config.getInt (path, key)
	return UZAPI.Config.GetInt(path, key);
end

--[[ 取得 變數 ]] -- float
function Config.getFloat (path, key)
	return UZAPI.Config.GetFloat(path, key);
end

--[[ 取得 變數 ]] -- bool
function Config.getBool (path, key)
	return UZAPI.Config.GetBool(path, key);
end

--[[ 取得 物件 ]] -- table
function Config.getTable (path, key)
	local obj_str = UZAPI.Config.GetStr(path, key);
	local obj = Json.decode(obj_str);
	return obj;
end


return Config;