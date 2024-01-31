local this = {};

--[[ 
	* 注意事項
	1. task的func中, 參數 會先有 ctrlr, 其後才是其他參數.
	2. task的func中, 要繼續記得手動next, 否則 視為中止不繼續.
]]

this._key2inst = {};

function this.del (key)
	if (key == nil) then key = "_default" end
	if (Util.containsKey(this._key2inst, key) == false) then return end

	this._key2inst[key].isDel = true;
	this._key2inst[key] = nil;
end

function this.inst (key)

	if (key == nil) then key = "_default" end
	if (Util.containsKey(this._key2inst, key)) then
		return this._key2inst[key];
	end

	local doPass = {};
	this._key2inst[key] = doPass;

	-- 任務列表
	doPass._tasks = {};

	-- 最後任務
	doPass._final = nil;

	-- 以標籤取得 所有符合標籤的
	doPass.gets = function (...)
		return this._gets(doPass, ...);
	end

	-- 新增
	doPass.add = function (task)
		if (Util.contains(doPass._tasks, task)) then return end
		doPass._tasks[#doPass._tasks+1] = task;
		doPass.sort();
	end

	-- 移除
	doPass.remove = function (...)
		local tags = {...};
		for idx = #doPass._tasks, 1, -1 do
			local each = doPass._tasks[idx];
			if (each.isTags(tags)) then
				Util.remove(doPass._tasks, each);
			end
		end
	end

	-- 清除
	doPass.clear = function ()
		Util.clear(doPass._tasks);
		doPass._final = nil;
	end

	-- 執行
	doPass.run = function (...)
		this.runDoPass(doPass, ...);
	end

	-- 設置最終
	doPass.final = function (finalFunc)
		doPass._final = finalFunc;
	end

	-- 排序
	doPass.sort = function ()
		if (#doPass._tasks < 2) then return end
		table.sort(doPass._tasks, function (a, b)
			return a._priority > b._priority;
		end);
	end

	return doPass;
end

function this.task ()

	local task = {};

	-- 標籤
	task._tags = {};

	-- 執行內容
	task._func = nil;

	-- 優先度
	task._priority = 0;

	-- 執行
	task.doFunc = function (passCtrlr, ...)
		task._func(passCtrlr, ...);
	end

	-- 設置 執行內容
	task.func = function (func)
		task._func = func;
		return task;
	end

	-- 設置 優先度
	task.pri = function (priority)
		task._priority = priority;
		return task;
	end

	-- 設置 標籤
	task.tags = function (...)
		Util.tags(task._tags, ...);
		return task;
	end

	-- 是否含有標籤
	task.isTags = function (tags)
		return Util.isTags(task._tags, tags);
	end

	return task;
end

-- 以標籤取得
function this._gets (doPass, ...)
	local res = {};
	for idx = 1, #doPass._tasks do
		local eachTask = doPass._tasks[idx];
		if (Util.isTags(eachTask._tags, {...})) then
			res[#res+1] = eachTask;
		end
	end
	return res;
end

-- 執行DoPass
function this.runDoPass (doPass, ...)
	local data = {...};

	-- 非同步任務
	local asyncTasks = {};
	-- 首個非同步任務 用來帶入參數
	asyncTasks[1] = function (next)
		next(nil, unpack(data));
	end

	-- 是否跳過
	local isSkip = false;
	-- 呼叫跳過
	local skip = function (...)
		if (doPass._final == nil) then return end
		doPass._final(...);
		isSkip = true;
	end

	for idx = 1, #doPass._tasks do

		local eachTask = doPass._tasks[idx];

		-- 每個非同步任務
		local asyncTask = function (...)

			-- 若 該執行通道 已經移除 則 返回
			if (doPass.isDel) then return end
			-- 若 執行過程 已經跳過 則 返回
			if (isSkip) then return end

			-- 取出 最後一個參數 作為 next
			local args = {...};
			local next = table.remove(args, #args);

			-- 控制項
			local passCtrlr = {
				["next"] = function (...)
					local args = {...};
					next(nil, unpack(args));
				end,
				["skip"] = skip
			};

			-- 執行 通道任務
			eachTask.doFunc(passCtrlr, unpack(args));
		end
		-- 加入 非同步任務
		asyncTasks[#asyncTasks+1] = asyncTask;
	end

	-- 執行非同步任務
	Async.waterfall(asyncTasks, function (...)
		if (doPass._final == nil) then return end
		local args = {...};
		table.remove(args, 1);
		doPass._final(unpack(args));
	end);
end


-- 測試
function this.test ()

	local doPass = DoPass.inst("_test");

	doPass.add(DoPass.task().func(function (passCtrlr)
		print("should been removed");
		passCtrlr.next();
	end).pri(2).tags("A", "B"));

	doPass.add(DoPass.task().func(function (passCtrlr)
		print("should been skiped");
		passCtrlr.next();
	end).pri(1).tags("A"));

	doPass.add(DoPass.task().func(function (passCtrlr, a, b)
		print(1);
		passCtrlr.next(a, b);
	end).pri(4).tags("A"));

	doPass.add(DoPass.task().func(function (passCtrlr, a, b)
		print(2);
		passCtrlr.skip(a..b);
	end).pri(3).tags("A"));

	doPass.final(function (msg)
		print(msg)
	end)


	doPass.remove(doPass.gets("A", "B"));

	doPass.run("hello", " world");

	DoPass.del("_test");

end


return this;
