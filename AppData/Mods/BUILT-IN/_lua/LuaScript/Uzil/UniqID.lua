local this = {};

this.group2ids = {}

function this.inst (group)
	local instance = {};

	local idList = nil;
	if (Util.contains(this.group2ids, group) == false) then
		idList = {};
		this.group2ids[group] = idList;
	else
		idList = this.group2ids[group];
	end

	--[[ ID列表 ]]
	instance.idList = idList

	--[[ 請求 ]]
	instance.request = function()

		local id = 0;
		while (Util.contains(idList, id)) do
			id = id + 1;
		end

		idList[#idList+1] = id;

		return id;

	end

	--[[ 釋放 ]]
	instance.release = function(id)
		if (Util.contains(idList, id) == false) then
			return;
		end
		Util.remove(idList, id);
	end

	return instance;

end


return this;