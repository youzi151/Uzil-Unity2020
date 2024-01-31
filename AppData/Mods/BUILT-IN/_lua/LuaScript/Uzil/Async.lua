local this = {};


--[[ 每個 ]]
-- 依序執行、不等候，直到執行次數達到全部成員數
function this.each (elements, eachFunc, onDone)
	local _onDone = function (res)
		if (onDone ~= nil) then onDone(res); end
	end

	local count = #elements;

	if (count == 0) then _onDone(); return; end

	for idx = 1, #elements do
		eachFunc(
			--[[ element ]]
			elements[idx],
			--[[ onEachDone ]]
			function ()
				count = count-1;
				if (count == 0) then
					_onDone();
				end
			end
		);
	end
end

--[[ 每個 ]]
-- 依序執行、逐個等候，直到執行次數達到全部成員數
function this.eachSeries (elements, eachFunc, onDone)
	local _onDone = function (res)
		if (onDone ~= nil) then onDone(res); end
	end

	local count = #elements;

	local nextTask = nil;
	nextTask = function (idx)

		-- 該序號的執行內容
		local element = elements[idx];

		-- 下一個序號
		local nextIdx = idx+1;

		-- 是否已經結束
		local isEnd = (nextIdx > count);

		-- 是否已經呼叫
		local isCalled = false;

		-- 加入next
		local toNext = function()
			-- 若 已經呼叫過 則 忽略
			if (isCalled) then return end
			isCalled = true;

			-- 若 還沒結束 則 執行下一個序號
			if (not isEnd) then
				nextTask(nextIdx);
			-- 否則 呼叫 結束 並 返回
			else
				_onDone();
			end
		end;

		-- 執行內容
		eachFunc(element, toNext);
	end;

	nextTask(1);
end

--[[ 過濾 ]]
-- 會依序執行不等候，直到執行次數達到全部成員數
function this.filter (elements, eachFunc, onDone)
	local _onDone = function (res)
		if (onDone ~= nil) then onDone(res); end
	end

	local count = #elements;
	local results = {};

	if (count == 0) then _onDone(); return; end

	for idx = 1, #elements do
		eachFunc(
			--[[ element ]]
			elements[idx],
			--[[ onEachDone ]]
			function (res)

				if (res ~= nil) then
					results[idx] = res;
				end

				count = count-1;
				if (count == 0) then
					_onDone(results);
				end
			end
		);
	end
end

--[[ 循環次數 ]]
-- 執行數次、每次不等候，直到執行次數達到指定次數
function this.times (count, eachFunc, onDone)
	local _onDone = function ()
		if (onDone ~= nil) then onDone(); end
	end

	if (count == 0) then _onDone(); return; end

	for idx = 1, count do
		eachFunc(
			idx,
			function ()
				count = count-1;
				if (count <= 0) then
					_onDone();
				end
			end
		);
	end
end

--[[ 循環次數 ]]
-- 執行數次、每次等候，直到執行次數達到指定次數
function this.timesSeries (count, eachFunc, onDone)
	local _onDone = function ()
		if (onDone ~= nil) then onDone(); end
	end

	local nextTask = nil;
	nextTask = function (idx)
		-- 下一個序號
		local nextIdx = idx+1;
		-- 是否已經呼叫
		local isCalled = false;
		-- 加入next
		local toNext = function()
			-- 若 已經呼叫過 則 忽略
			if (isCalled) then return end
			isCalled = true;
			-- 若 還沒結束 則 執行下一個序號
			if (nextIdx <= count) then
				nextTask(nextIdx);
			-- 否則 呼叫 結束 並 返回
			else
				_onDone();
			end
		end;
		-- 執行內容
		eachFunc(idx, toNext);
	end;

	nextTask(1);
end


--[[ 依序執行 ]]
function this.waterfall (tasks, onDone)

	local nextTask = nil;

	nextTask = function (idx, ...)
		local args = {...};

		-- 該序號的執行內容
		local task = tasks[idx];

		-- 下一個序號
		local nextIdx = idx+1;

		-- 是否已經結束
		local isEnd = (nextIdx > #tasks);

		-- 是否已經呼叫
		local isCalled = false;

		-- 加入next
		local toNext = function(...)
			-- 若 已經呼叫過 則 忽略
			if (isCalled) then return end
			isCalled = true;

			local toNextArgs = {...};
			if (toNextArgs[1] ~= nil) then
				isEnd = true;
			end
			table.remove(toNextArgs, 1);

			-- 若 已經結束 則 呼叫 結束 並 返回
			if (isEnd) then
				if (onDone ~= nil) then onDone(...); end
				-- 否則 執行下一個序號
			else
				nextTask(nextIdx, unpack(toNextArgs));
			end
		end;
		args[#args+1] = toNext;

		-- 執行內容
		task(unpack(args));

	end;

	nextTask(1);

end

--[[ 一齊執行 ]]
function this.parallel (tasks, onDone)

	local isStop = false;

	local leftTask = #tasks;

	local eachDone = function (idx)
		if (isStop) then return end

		leftTask = leftTask - 1;
		if (leftTask <= 0)  then
			onDone();
		end
	end

	for idx = 1, #tasks do

		local eachTask = tasks[idx];

		local isDone = false;

		local toIdx = idx;

		eachTask(function ()
			if (isDone) then return end
			isDone = true;
			eachDone(toIdx);
		end);
	end

end

function this.test_waterfall ()
	this.waterfall(
		{
			function (next)
				print("start");
				next("1");
			end,

			function (log1, next)
				print(log1);
				next(log1, "2");
			end,

			function (log1, log2, next)
				print(log1..log2)
				next();
			end,

		},
		function ()
			print("done");
		end
	);
end

return this;