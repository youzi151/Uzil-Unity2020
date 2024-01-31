local this = {};

--[[ 取得實例 ]] -- table
function this.inst (key)
	local inst = {};

	inst.key = key;

	--[[ 是否存在 ]] -- bool
	inst.isExist = function (key)
		return this.isExist(inst.key, key);
	end

	--[[ 設置 數字 ]] -- void
	inst.setNum = function (key, val)
		this.setNum(inst.key, key, val);
	end
	--[[ 設置 字串 ]] -- void
	inst.setStr = function (key, val)
		this.setStr(inst.key, key, val);
	end
	--[[ 設置 布林 ]] -- void
	inst.setBool = function (key, val)
		this.setBool(inst.key, key, val);
	end
	--[[ 設置 物件 (以字串變數保存) ]] -- void
	inst.setTable = function (key, val)
		this.setTable(inst.key, key, val);
	end

	--[[ 取得 數字 ]] -- number
	inst.getFloat = function (key)
		return this.getFloat(inst.key, key);
	end
	--[[ 取得 數字 ]] -- number
	inst.getInt = function (key)
		return this.getInt(inst.key, key);
	end
	--[[ 取得 字串 ]] -- string
	inst.getStr = function (key)
		return this.getStr(inst.key, key);
	end
	--[[ 取得 布林 ]] -- string
	inst.getBool = function (key)
		return this.getBool(inst.key, key);
	end
	--[[ 取得 物件 (從字串變數) ]]
	inst.getTable = function (key)
		return this.getTable(inst.key, key);
	end

	--[[ 移除 數字 ]] -- void
	inst.delNum = function (key)
		this.delNum(inst.key, key);
	end
	--[[ 移除 字串 ]] -- void
	inst.delStr = function (key)
		this.delStr(inst.key, key);
	end
	--[[ 移除 布林 ]] -- void
	inst.delBool = function (key)
		this.delBool(inst.key, key);
	end
	--[[ 移除 任意 ]] -- void
	inst.del = function (key)
		this.del(inst.key, key);
	end

	return inst;
end



--[[ 是否存在 ]] -- bool
function this.isExist (instanceKey, key)
	return tobool(UZAPI.Var.IsExist(instanceKey, key));
end

--[[ 設置 數字 ]] -- void
function this.setNum (instanceKey, key, val)
	UZAPI.Var.SetNum(instanceKey, key, val);
end

--[[ 設置 字串 ]] -- void
function this.setStr (instanceKey, key, val)
	UZAPI.Var.SetStr(instanceKey, key, val);
end

--[[ 設置 布林 ]]
function this.setBool (instanceKey, key, val)
	UZAPI.Var.SetBool(instanceKey, key, val);
end

--[[ 設置 物件 (以字串變數保存) ]] -- void
function this.setTable (instanceKey, key, val)
	UZAPI.Var.SetStr(instanceKey, key, Json.encode(val));
end

--[[ 取得 數字 ]] -- number
function this.getInt (instanceKey, key)
	if (this.isExist(instanceKey, key) == false) then return nil end
	return UZAPI.Var.GetInt(instanceKey, key);
end

--[[ 取得 數字 ]] -- number
function this.getFloat (instanceKey, key)
	if (this.isExist(instanceKey, key) == false) then return nil end
	return UZAPI.Var.GetFloat(instanceKey, key);
end

--[[ 取得 字串 ]] -- string
function this.getStr (instanceKey, key)
	if (this.isExist(instanceKey, key) == false) then return nil end
	return UZAPI.Var.GetStr(instanceKey, key);
end

--[[ 取得 布林 ]] -- string
function this.getBool (instanceKey, key)
	if (this.isExist(instanceKey, key) == false) then return nil end
	return UZAPI.Var.GetBool(instanceKey, key);
end

--[[ 取得 物件 (從字串變數) ]]
function this.getTable (instanceKey, key)
	if (this.isExist(instanceKey, key) == false) then return nil end
	return Json.decode(UZAPI.Var.GetStr(instanceKey, key));
end

--[[ 移除 數字 ]] -- void
function this.delNum (instanceKey, key)
	UZAPI.Var.DelNum(instanceKey, key);
end

--[[ 移除 字串 ]] -- void
function this.delStr (instanceKey, key)
	UZAPI.Var.DelStr(instanceKey, key);
end

--[[ 移除 布林 ]] -- void
function this.delBool (instanceKey, key)
	UZAPI.Var.DelBool(instanceKey, key);
end

--[[ 移除 任意 ]] -- void
function this.del (instanceKey, key)
	UZAPI.Var.Del(instanceKey, key);
end

return this;