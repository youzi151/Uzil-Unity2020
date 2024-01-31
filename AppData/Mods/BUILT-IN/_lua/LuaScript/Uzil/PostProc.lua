local this = {};

function this.inst (key)

	local inst = {};
	inst.key = key;

	--[[ 設置 優先(越小越先) ]]
	inst.setPriority = function (userName, priority)
		this.setPriority(inst.key, userName, priority);
	end

	--[[ 設置效果 ]]
	inst.setEffects = function (userName, effects)
		this.setEffects(inst.key, userName, effects);
	end

	--[[ 移除效果 ]]
	inst.removeEffects = function (userName)
		this.removeEffects(inst.key, userName);
	end

	--[[ 更新 效果 ]]
	inst.updateEffects = function ()
		this.updateEffects(inst.key);
	end

	return inst;
end


--[[ 設置 優先(越小越先) ]]
function this.setPriority (key, userName, priority)
	UZAPI.PostProc.SetPriority(key, userName, priority);
end

--[[ 設置 效果 ]]
function this.setEffects (key, userName, effects)
	UZAPI.PostProc.SetEffects(key, userName, Json.encode(effects));
end

--[[ 移除 效果 ]]
function this.removeEffects (key, userName)
	UZAPI.PostProc.RemoveEffects(key, userName);
end

--[[ 更新 效果 ]]
function this.updateEffects (key)
	UZAPI.PostProc.UpdateEffects(key);
end


return this;