local this = {};

--[[ 區塊類型 ]]
this.BlockType = {
	["empty"] = "Empty",
	["text"] = "Text",
	["image"] = "Image",
	["button"] = "Button",
	["toggle"] = "Toggle",
	["inputField"] = "InputField",
	["select"] = "Select",
	["switch"] = "Switch",
	["slider"] = "Slider",
};

--[[ 取得實例 ]] -- table
function this.inst(pageID)

	local instance = {};

	instance.pageID = pageID;

	--[[ 新增 區塊 ]] -- void
	instance.addBlock = function (blockID, blockType, data)
		this.addBlock(instance.pageID, blockID, blockType, data);
	end

	--[[ 新增 區塊 ]]
	instance.insertBlock = function (rowIdx, blockID, blockType, data)
		this.insertBlock(instance.pageID, rowIdx, blockID, blockType, data);
	end

	--[[ 設置 區塊 ]]
	instance.setBlock = function (blockID, data)
		this.setBlock(instance.pageID, blockID, data);
	end

	--[[ 移除 區塊 ]]
	instance.removeBlock = function (blockID)
		this.removeBlock(instance.pageID, blockID);
	end

	--[[ 清除 所有區塊 ]]
	instance.clearBlocks = function ()
		this.clearBlocks(instance.pageID);
	end

	--[[ 移除 行 ]]
	instance.removeRow = function (rowIdx)
		this.removeRow(instance.pageID, rowIdx);
	end

	--[[ 更新 排版 ]]
	instance.updateLayout = function ()
		this.updateLayout(instance.pageID);
	end

	return instance;
end

--[[ 新增 區塊 ]] -- void
function this.addBlock (pageID, blockID, blockType, data)
	UZAPI.BlockPage.AddBlock(pageID, blockID, blockType, Json.encode(data));
end

--[[ 新增 區塊 ]]
function this.insertBlock (pageID, rowIdx, blockID, blockType, data)
	UZAPI.BlockPage.InsertBlock(pageID, rowIdx, blockID, blockType, Json.encode(data));
end

--[[ 設置 區塊 ]]
function this.setBlock (pageID, blockID, data)
	UZAPI.BlockPage.SetBlock(pageID, blockID, Json.encode(data));
end

--[[ 移除 區塊 ]]
function this.removeBlock (pageID, blockID)
	UZAPI.BlockPage.RemoveBlock(pageID, blockID);
end

--[[ 清除 所有區塊 ]]
function this.clearBlocks (pageID)
	UZAPI.BlockPage.ClearBlocks(pageID);
end

--[[ 移除 行 ]]
function this.removeRow (pageID, rowIdx)
	UZAPI.BlockPage.RemoveRow(pageID, rowIdx);
end

--[[ 更新 排版 ]]
function this.updateLayout (pageID)
	UZAPI.BlockPage.UpdateLayout(pageID);
end

return this;