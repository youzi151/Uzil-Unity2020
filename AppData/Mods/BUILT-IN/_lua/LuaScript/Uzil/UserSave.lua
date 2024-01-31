local UserSave = {};

--[[
	優先使用ProfileSave
	UserSave 為 提供 跨Profile之資料 儲存所用

	UserSave is used to save cross-profile data.
	Use ProfileSave is more suitable usually.
]]

function UserSave.inst (path)

	local this = {};
	this.path = path;

	this.isExist = function (key)
		return UserSave.isExist(this.path, key);
	end

	this.setStr = function (key, val)
		return UserSave.setStr(this.path, key, val);
	end

	this.setNum = function (key, val)
		return UserSave.setNum(this.path, key, val);
	end

	this.setBool = function (key, val)
		return UserSave.setBool(this.path, key, val);
	end

	this.setTable = function (key, val)
		return UserSave.setTable(this.path, key, val);
	end

	this.getStr = function (key)
		return UserSave.getStr(this.path, key);
	end

	this.getInt = function (key)
		return UserSave.getInt(this.path, key);
	end

	this.getFloat = function (key)
		return UserSave.getFloat(this.path, key);
	end

	this.getBool = function (key)
		return UserSave.getBool(this.path, key);
	end

	this.getTable = function (key)
		return UserSave.getTable(this.path, key);
	end

	this.remove = function (key)
		return UserSave.remove(this.path, key);
	end

	this.delete = function ()
		return UserSave.delete(this.path);
	end

	return this;
end
UserSave.file = UserSave.inst;

--[[ 是否 存在 ]] -- bool
function UserSave.isExist (path, key)
   return tobool(UZAPI.UserSave.IsExist(path, key));
end

--[[ 設置 字串 ]] -- bool
function UserSave.setStr (path, key, val)
	return tobool(UZAPI.UserSave.SetStr(path, key, val));
end

--[[ 設置 數字 ]] -- bool
function UserSave.setNum (path, key, val)
	return tobool(UZAPI.UserSave.SetNum(path, key, val));
end

--[[ 設置 布林 ]] -- bool
function UserSave.setBool (path, key, val)
	return tobool(UZAPI.UserSave.SetBool(path, key, val));
end

--[[ 設置 物件 ]] -- bool
function UserSave.setTable (path, key, val)
	if (Util.isArray(val)) then
		return tobool(UZAPI.UserSave.SetList(path, key, Json.encode(val)));
	else
		return tobool(UZAPI.UserSave.SetObj(path, key, Json.encode(val)));
	end
end

--[[ 取得 字串 ]] -- string
function UserSave.getStr (path, key)
	return UZAPI.UserSave.GetStr(path, key);
end

--[[ 取得 變數 ]] -- int
function UserSave.getInt (path, key)
	return UZAPI.UserSave.GetInt(path, key);
end

--[[ 取得 變數 ]] -- float
function UserSave.getFloat (path, key)
	return UZAPI.UserSave.GetFloat(path, key);
end


--[[ 取得 變數 ]] -- bool
function UserSave.getBool (path, key)
	return UZAPI.UserSave.GetBool(path, key);
end

--[[ 取得 物件 ]] -- table
function UserSave.getTable (path, key)
	local obj_str = UZAPI.UserSave.GetStr(path, key);
	local obj = Json.decode(obj_str);
	return obj;
end

--[[ 移除 ]] -- bool
function UserSave.remove (path, key)
	return UZAPI.UserSave.Remove(path, key);
end

--[[ 刪除 ]] -- bool
function UserSave.delete (path)
	return UZAPI.UserSave.Delete(path);
end


return UserSave;