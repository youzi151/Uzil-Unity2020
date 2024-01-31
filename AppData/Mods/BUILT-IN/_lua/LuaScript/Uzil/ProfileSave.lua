local ProfileSave = {};

function ProfileSave.inst (path)

	local this = {};
	this.path = path;

	this.isExist = function (key)
		return ProfileSave.isExist(this.path, key);
	end

	this.setStr = function (key, val)
		return ProfileSave.setStr(this.path, key, val);
	end

	this.setNum = function (key, val)
		return ProfileSave.setNum(this.path, key, val);
	end

	this.setBool = function (key, val)
		return ProfileSave.setBool(this.path, key, val);
	end

	this.setTable = function (key, val)
		return ProfileSave.setTable(this.path, key, val);
	end

	this.getStr = function (key)
		return ProfileSave.getStr(this.path, key);
	end

	this.getInt = function (key)
		return ProfileSave.getInt(this.path, key);
	end

	this.getFloat = function (key)
		return ProfileSave.getFloat(this.path, key);
	end

	this.getBool = function (key)
		return ProfileSave.getBool(this.path, key);
	end

	this.getTable = function (key)
		return ProfileSave.getTable(this.path, key);
	end

	this.remove = function (key)
		return ProfileSave.remove(this.path, key);
	end

	this.delete = function ()
		return ProfileSave.delete(this.path);
	end

	return this;
end
ProfileSave.file = ProfileSave.inst;

--[[ 是否 存在 ]] -- bool
function ProfileSave.isExist (path, key)
   return tobool(UZAPI.ProfileSave.IsExist(path, key));
end

--[[ 設置 字串 ]] -- bool
function ProfileSave.setStr (path, key, val)
	return tobool(UZAPI.ProfileSave.SetStr(path, key, val));
end

--[[ 設置 數字 ]] -- bool
function ProfileSave.setNum (path, key, val)
	return tobool(UZAPI.ProfileSave.SetNum(path, key, val));
end

--[[ 設置 布林 ]] -- bool
function ProfileSave.setBool (path, key, val)
	return tobool(UZAPI.ProfileSave.SetBool(path, key, val));
end

--[[ 設置 物件 ]] -- bool
function ProfileSave.setTable (path, key, val)
	if (Util.isArray(val)) then
		return tobool(UZAPI.ProfileSave.SetList(path, key, Json.encode(val)));
	else
		return tobool(UZAPI.ProfileSave.SetObj(path, key, Json.encode(val)));
	end
end

--[[ 取得 字串 ]] -- string
function ProfileSave.getStr (path, key)
	return UZAPI.ProfileSave.GetStr(path, key);
end

--[[ 取得 變數 ]] -- int
function ProfileSave.getInt (path, key)
	return UZAPI.ProfileSave.GetInt(path, key);
end

--[[ 取得 變數 ]] -- float
function ProfileSave.getFloat (path, key)
	return UZAPI.ProfileSave.GetFloat(path, key);
end


--[[ 取得 變數 ]] -- bool
function ProfileSave.getBool (path, key)
	return UZAPI.ProfileSave.GetBool(path, key);
end

--[[ 取得 物件 ]] -- table
function ProfileSave.getTable (path, key)
	local obj_str = UZAPI.ProfileSave.GetStr(path, key);
	local obj = Json.decode(obj_str);
	return obj;
end

--[[ 移除 ]] -- bool
function ProfileSave.remove (path, key)
	return UZAPI.ProfileSave.Remove(path, key);
end

--[[ 刪除 ]] -- bool
function ProfileSave.delete (path)
	return UZAPI.ProfileSave.Delete(path);
end


return ProfileSave;