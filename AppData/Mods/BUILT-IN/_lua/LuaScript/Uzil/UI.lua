local this = {};

function this.inst (key)

	local inst = {};
	inst.key = key;

	UZAPI.UI.Inst(key);

	--[[ 銷毀 ]]
	inst.destroyInst = function ()
		this.destroyInst(inst.key);
	end

	--[[ 排序 ]]
	inst.sortInst = function (sort)
		this.sortInst(inst.key, sort);
	end

	--[[ 取得Canvas尺寸 ]]
	inst.getCanvasSize = function ()
		return this.getCanvasSize(inst.key);
	end

	--[[ 取得Canvas參考尺寸 ]]
	inst.getCanvasRefSize = function ()
		return this.getCanvasRefSize(inst.key);
	end

	--[[ 設置渲染模式 ]]
	inst.setRenderMode = function (renderMode)
		this.setRenderMode(inst.key, renderMode);
	end

	--[[ 設置 資料 ]]
	inst.create = function (id, data)
		this.create(inst.key, id, data);
	end

	--[[ 銷毀 ]]
	inst.remove = function (id)
		this.remove(inst.key, id);
	end

	--[[ 清空 ]]
	inst.clear = function ()
		this.clear(inst.key);
	end

	--[[ 強制更新 ]]
	inst.forceUpdate = function (id)
		this.forceUpdate(inst.key, id);
	end

	--[[ 排序 ]]
	inst.sort = function (id, sort)
		this.sort(inst.key, id, sort);
	end

	--[[ 設置 資料 ]]
	inst.setData = function (id, data)
		this.setData(inst.key, id, data);
	end

	--[[ 設置 位置 ]]
	inst.setPosition = function (id, pos_v2)
		this.setPosition(inst.key, id, pos_v2);
	end

	--[[ 取得 資料 ]]
	inst.getData = function (id, props)
		return this.getData(inst.key, id, props);
	end

	--[[ 設置 屬性 ]]
	inst.setProp = function (id, prop, value)
		this.setProp(inst.key, id, prop, value);
	end

	--[[ 設置 圖像資料 ]]
	inst.setImageData = function (id, data)
		this.setImageData(inst.key, id, data);
	end

	--[[ 設置 圖像屬性 ]]
	inst.setImageProp = function (id, prop, value)
		this.setImageProp(inst.key, id, prop, value);
	end

	--[[ 設置 文字資料 ]]
	inst.setTextData = function (id, data)
		this.setTextData(inst.key, id, data);
	end

	--[[ 設置 文字屬性 ]]
	inst.setTextProp = function (id, prop, value)
		this.setTextProp(inst.key, id, prop, value);
	end

	return inst;
end

--[[ 銷毀 ]]
function this.destroyInst (key)
	UZAPI.UI.DestroyInst(key);
end

--[[ 排序 ]]
function this.sortInst (key, sort)
	UZAPI.UI.SortInst(key, sort);
end

--[[ 取得Canvas尺寸 ]]
function this.getCanvasSize (key)
	return Json.decode(UZAPI.UI.GetCanvasSize(key));
end

--[[ 取得Canvas參考尺寸 ]]
function this.getCanvasRefSize (key)
	return Json.decode(UZAPI.UI.GetCanvasRefSize(key));
end

--[[ 設置渲染模式 ]]
function this.setRenderMode (key, renderMode)
	UZAPI.UI.SetRenderMode(key, renderMode);
end

--[[ 建立 ]]
function this.create (key, id, data)
	UZAPI.UI.Create(key, id, Json.encode(data));
end

--[[ 銷毀 ]]
function this.remove (key, id)
	UZAPI.UI.Remove(key, id);
end

--[[ 清空 ]]
function this.clear (key)
	UZAPI.UI.Clear(key);
end

--[[ 強制更新 ]]
function this.forceUpdate (key, id)
	UZAPI.UI.ForceUpdate(key, id);
end

--[[ 排序 ]]
function this.sort (key, id, sort)
	UZAPI.UI.Sort(key, id, sort);
end

--[[ 設置 資料 ]]
function this.setData (key, id, data)
	UZAPI.UI.SetData(key, id, Json.encode(data));
end

--[[ 取得 資料 ]]
function this.getData (key, id, props)
	return Json.decode(UZAPI.UI.GetData(key, id, Json.encode(props)));
end

--[[ 設置 位置 ]]
function this.setPosition (key, id, pos_v2)
	this.setData(key, id, {
		position = pos_v2
	});
end

--[[ 設置 屬性 ]]
function this.setProp (key, id, prop, value)
	if (type(value) == "table") then value = Json.encode(value) end
	UZAPI.UI.SetProp(key, id, prop, value);
end

--[[ 設置 圖像資料 ]]
function this.setImageData (key, id, data)
	UZAPI.UI.SetImageData(key, id, Json.encode(data));
end

--[[ 設置 圖像屬性 ]]
function this.setImageProp (key, id, prop, value)
	UZAPI.UI.SetImageProp(key, id, prop, value);
end

--[[ 設置 文字資料 ]]
function this.setTextData (key, id, data)
	UZAPI.UI.SetTextData(key, id, Json.encode(data));
end

--[[ 設置 文字屬性 ]]
function this.setTextProp (key, id, prop, value)
	UZAPI.UI.SetTextProp(key, id, prop, value);
end

--[[ 設置 動畫參數 ]]
function this.setAnimParam (key, id, paramName, value)
	UZAPI.UI.SetAnimParam(key, id, paramName, value);
end

return this;