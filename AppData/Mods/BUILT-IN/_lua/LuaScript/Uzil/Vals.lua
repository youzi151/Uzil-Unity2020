local Vals = {};

Vals.new = function ()

	local inst = {};

	--[[ 註冊表 ]]
	inst.users = {};

	--[[ 數量 ]]
	inst._count = 0;

	--[[ 當前值 ]]
	inst._current = nil;

	--[[ 當前值 ]]
	inst._currentUser = nil;

	--[[ 取得數量 ]]
	inst.getCount = function ()
		return inst._count;
	end

	--[[ 取得預設值 ]]
	inst.getDefault = function ()
		return nil;
	end

	--[[ 取得當前值 ]]
	inst.getCurrent = function ()
		return Util.isOr(inst._current ~= nil, inst._current, inst.getDefault());
	end;

	--[[ 取得當前值 ]]
	inst.getCurrentUser = function ()
		return inst._currentUser;
	end;

	--[[ 設置 ]]
	inst.set = function (user, value, priority, isTemp)
		Vals.set(inst, user, value, priority, isTemp);
	end

	--[[ 取得 ]]
	inst.get = function (user)
		return Vals.get(inst, user);
	end

	--[[ 移除 ]]
	inst.del = function (user)
		Vals.del(inst, user);
	end

	--[[ 清除 ]]
	inst.clear = function ()
		Vals.clear(inst);
	end

	--[[ 更新 ]]
	inst.update = function ()
		Vals.update(inst);
	end

	--[[ 匯出 ]]
	inst.save = function (isCopy)
		return Vals.save(inst, isCopy);
	end

	--[[ 匯入 ]]
	inst.load = function (save, isCopy)
		Vals.load(inst, save, isCopy);
	end

	return inst;
end

Vals.set = function (inst, user, value, priority, isTemp)
	local exist = inst.users[user];
	if (exist ~= nil) then

		if (priority == nil) then
			priority = exist.pri;
		end

		if (exist.val == value and exist.pri == priority) then
			return;
		end

		exist.val = value;
		exist.pri = priority;
		exist.tmp = isTemp;

	else

		inst.users[user] = {
			["val"] = value,
			["pri"] = priority,
			["tmp"] = isTemp
		};

		inst._count = inst._count + 1;

	end
	inst.update();
end

Vals.get = function (inst, user)
	local exist = inst.users[user];
	if (exist == nil) then return nil end

	return exist.val;
end

Vals.del = function (inst, user)
	if (Util.containsKey(inst.users, user) == false) then return end
	Util.remove(inst.users, user);
	inst._count = inst._count - 1;
	inst.update();
end

Vals.clear = function (inst)
	inst.users = {};
	inst._count = 0;
	inst._current = nil;
	inst._currentUser = nil;
	inst.getDefault = function ()
		return nil;
	end
end

Vals.update = function (inst)
	local mostPri = 0
	local mostVal = nil;

	Util.each(inst.users, function (v_info, k_user)
		if (mostVal == nil or v_info.pri > mostPri) then
			mostVal = v_info.val;
			mostPri = v_info.pri;

			if (not v_info.tmp) then
				inst._currentUser = k_user;
			end
		end
	end);

	inst._current = mostVal;
end

Vals.save = function (inst, isCopy)
	return {
		["users"] = Util.isOr(isCopy, Util.deepCopy(inst.users), inst.users)
	};
end

Vals.load = function (inst, save, isCopy)
	if (save == nil) then return end
	local users = save["users"];
	if (users ~= nil) then
		inst.users = Util.isOr(isCopy, Util.deepCopy(users), users);
	end
end

return Vals;