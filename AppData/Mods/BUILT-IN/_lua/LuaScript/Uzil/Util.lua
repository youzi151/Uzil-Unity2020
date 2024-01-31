local this = {};

--  ######   #######  ##    ## ##     ## ######## ########  ######## 
-- ##    ## ##     ## ###   ## ##     ## ##       ##     ##    ##    
-- ##       ##     ## ####  ## ##     ## ##       ##     ##    ##    
-- ##       ##     ## ## ## ## ##     ## ######   ########     ##    
-- ##       ##     ## ##  ####  ##   ##  ##       ##   ##      ##    
-- ##    ## ##     ## ##   ###   ## ##   ##       ##    ##     ##    
--  ######   #######  ##    ##    ###    ######## ##     ##    ##    

--[[ 轉換為布林 ]]
toboolean = function (obj)
	if (obj == nil) then return nil
	elseif (obj == "true") then return true;
	elseif (obj == "True") then return true;
	elseif (obj == 1) then return true;
	elseif (obj == true) then return true;
	elseif (obj == "1") then return true ;
	elseif (obj == "false") then return false;
	elseif (obj == "False") then return false;
	elseif (obj == 0) then return false;
	elseif (obj == false) then return false;
	elseif (obj == "0") then return false;
	else return nil
	end
end
tobool = toboolean;

-- ##     ## ##    ## ####  #######  
-- ##     ## ###   ##  ##  ##     ## 
-- ##     ## ####  ##  ##  ##     ## 
-- ##     ## ## ## ##  ##  ##     ## 
-- ##     ## ##  ####  ##  ##  ## ## 
-- ##     ## ##   ###  ##  ##    ##  
--  #######  ##    ## ####  ##### ##  

--[[ 請求一個新的唯一ID ]]
this.dispatchID = 0;
function this.getUniq()
	local res = "[dispatch-"..this.dispatchID.."]";
	this.dispatchID = this.dispatchID+1;
	return res;
end


--  ######  ######## ########  #### ##    ##  ######   
-- ##    ##    ##    ##     ##  ##  ###   ## ##    ##  
-- ##          ##    ##     ##  ##  ####  ## ##        
--  ######     ##    ########   ##  ## ## ## ##   #### 
--       ##    ##    ##   ##    ##  ##  #### ##    ##  
-- ##    ##    ##    ##    ##   ##  ##   ### ##    ##  
--  ######     ##    ##     ## #### ##    ##  ######   
--
--[[ 是否起始為 ]]
function this.startWith (str, start)
	return string.sub(str, 1, string.len(start)) == start;
end

--[[ 是否結束為 ]]
function this.endWith (str, _end)
	return _end == '' or string.sub(str, -string.len(_end)) == _end;
end

--[[ 字串雜湊至數字 ]]
function this.getStringHashCode (str)
	return UZAPI.Util.GetStringHashCode(str);
end

--[[ 分拆字串 ]]
function this.splitStr (inputstr, sep)
	if (sep == nil) then sep = "%s"; end
	local res = {};
	for str in string.gmatch(inputstr, "([^"..sep.."]+)") do
		table.insert(res, str)
	end
	return res;
end

--[[ 補齊字串 ]]
function this.padLeft (str, padChar, width)
	while (#str < width) do
		str = padChar..str;
	end
	return str
end
function this.padRight (str, padChar, width)
	while (#str < width) do
		str = str..padChar;
	end
	return str
end

-- ##        #######   ######   
-- ##       ##     ## ##    ##  
-- ##       ##     ## ##        
-- ##       ##     ## ##   #### 
-- ##       ##     ## ##    ##  
-- ##       ##     ## ##    ##  
-- ########  #######   ######   

local isPrintToLogFile = false;
if (isPrintToLogFile) then
	local _print = print;
	print = function (msg)
		_print(msg);
		local file = io.open("./print.log", "a");
		file:write(msg.."\n");
		file:close();
	end
	print("== GAMESTART =============\n"..os.date('%Y-%m-%d %H:%M:%S').."\n==========================");
end

-- ######## ##    ## ##     ## ##     ## 
-- ##       ###   ## ##     ## ###   ### 
-- ##       ####  ## ##     ## #### #### 
-- ######   ## ## ## ##     ## ## ### ## 
-- ##       ##  #### ##     ## ##     ## 
-- ##       ##   ### ##     ## ##     ## 
-- ######## ##    ##  #######  ##     ## 

--[[ 產生列舉 ]]
function this.enum (tb)
	local enumTable = {};
	for key, value in pairs(tb) do
		enumTable[value] = key;
		enumTable[key] = value
	end
	return enumTable;
end

--[[ 鍵 ]]
function this.enumKey (value, enum)
	return enum[value];
end

--[[ 鍵值列表 ]]
function this.enumKeys (tb)
	local keys = {};
	for i, v in ipairs(tb) do
		keys[#keys+1] = v;
	end
	return keys;
end



-- ########    ###    ########  ##       ########    
--    ##      ## ##   ##     ## ##       ##          
--    ##     ##   ##  ##     ## ##       ##          
--    ##    ##     ## ########  ##       ######      
--    ##    ######### ##     ## ##       ##          
--    ##    ##     ## ##     ## ##       ##          
--    ##    ##     ## ########  ######## ########    

--[[ 長度 ]]
function this.count (tbOrArray)
	if (type(tbOrArray) ~= "table") then return nil end
	if (this.isArray(tbOrArray)) then
		return #tbOrArray
	else
		local count = 0;
		for k, v in pairs(tbOrArray) do
			count = count+1;
		end
		return count;
	end
end

--[[ 是否包含 ]]
function this.contains (tb, _value)
	if (tb == nil) then return false end
	if (#tb == 0) then return false end

	if (type(tb) ~= "table") then return false end

	-- 若要判斷容器的內容類型 不是table 且 要判斷的內容卻是table，則另外處理
	if (type(tb[1]) ~= "table" and type(_value) == "table") then 
		for key, value in pairs(_value) do
			if (this.contains(tb, value) == false) then
				return false;
			end
		end
		return true;
	end

	for key, value in pairs(tb) do
		if (value == _value) then
			return true;
		end
	end

	return false;
end

--[[ 是否包含鍵值 ]]
function this.containsKey (tb, _key)
	if (tb == nil) then return false end
	for key, value in pairs(tb) do
		if key == _key then
			return true;
		end
	end
	return false;
end

--[[ 取得序號 ]]
function this.indexOf (tb, value)
	for i = 1, #tb do
		if (tb[i] == value) then return i end
	end
	return nil;
end

--[[ 搜尋 ]]
function this.findIdx (tb, isReturn)
	for i = 1, #tb do
		if (isReturn(tb[i])) then return i end
	end
	return nil;
end

--[[ 交集 ]]
function this.intersect (tableA, tableB)
	local res = {}
	if ((tableA == nil) or (tableB == nil)) then return {} end
	for keyA, valA in pairs(tableA) do
		for keyB, valB in pairs(tableB) do
			if (valB == valA) then
				res[#res+1] = valA;
			end
		end	
	end
	return res;
end

--[[ 新增 ]]
function this.add (tb, _value)
	local isArray = this.isArray(tb);
	if (isArray == false) then return end;

	tb[#tb+1] = _value;
end

--[[ 移除 ]]
function this.remove (tb, _valueOrKey)
	local isArray = this.isArray(tb);

	local tryRM;
	if (isArray) then
		tryRM = function (tb, key, value)
			if (value == _valueOrKey) then table.remove(tb, key) end
		end
	else
		tryRM = function (tb, key, value)
			if (key == _valueOrKey) then tb[key] = nil; end
		end
	end

	for key, value in pairs(tb) do
		tryRM(tb, key, value);
	end
end

--[[ 清空 ]]
function this.clear (tb)
	local isArray = this.isArray(tb);
	if (isArray) then
		for idx = #tb, 1, -1 do
			table.remove(tb, idx);
		end
	else
		for key, value in pairs(tb) do
			table[key] = nil;
		end
	end
end

--[[ 聯集 ]]
function this.union (...)
	local result = {};

	local args = {...};

	for i = 1, #args do
		local each = args[i];
		if (type(each) == "table") then
			if (this.isArray(each)) then
				for k,v in pairs(each) do
					table.insert(result, v);
				end
			else
				for k,v in pairs(each) do
					result[k] = v;
				end
			end
		else
			table.insert(result, each);
		end
	end

	return result;
end

--[[ 合併 ]]
function this.merge (...)
	local result = {};

	local args = {...};

	for i = 1, #args do while true do
		local each = args[i];
		if (each == nil) then break end --continue

		if (this.isArray(each)) then
			for k, v in pairs(each) do
				if (this.contains(result, v) == false) then
					result[#result+1] = v;
				end
			end
		else
			for k, v in pairs(each) do
				local exist = result[k];
				if (exist ~= nil and type(exist) == "table" and this.isArray(exist) == false) then
					result[k] = this.merge(exist, v);
				else
					result[k] = v;
				end
			end
		end
	break end end

	return result;

end

--[[ 每個 ]]
function this.each (tb, eachFunc)
	if (tb == nil) then return end
	local isArray = this.isArray(tb);

	local toEach = {};
	if (isArray == true) then
		for i = 1, #tb do
			local v = tb[i];
			toEach[#toEach+1] = {v, i};
		end
	else
		for k, v in pairs(tb) do
			toEach[#toEach+1] = {v, k};
		end
	end
	for idx = 1, #toEach do
		eachFunc(toEach[idx][1], toEach[idx][2]);
	end
end

--[[ 篩選 ]]
function this.filter (tb, pass)
	local res = {};

	local isArray = this.isArray(tb);

	if (isArray == true) then
		for i = 1, #tb do
			local v = tb[i];
			v = pass(v);
			if (v ~= nil) then
				res[#res+1] = v;
			end
		end
	else
		for k, v in pairs(tb) do
			local vv = pass(k, v);
			if (vv ~= nil) then
				res[k] = vv;
			end
		end
	end

	return res;
end

--[[ 是否為陣列 ]]
function this.isArray (tb)
	if (tb == nil) then return false end
	if (type(tb) ~= 'table') then return false end
	if (#tb > 0 and tb[1] == nil) then return false end
	for k, v in pairs(tb) do
		if (type(k) ~= 'number') then
			return false;
		end
	end

	return true;
end

--[[ 轉換單一為陣列 ]]
function this.oneToArray (oneOrArr)
	if (this.isArray(oneOrArr)) then
		return oneOrArr;
	else
		return {oneOrArr};
	end
end

--  ######    ######  
-- ##    ##  ##    ## 
-- ##        ##       
-- ##   #### ##       
-- ##    ##  ##       
-- ##    ##  ##    ## 
--  ######    ######  

function this.gcCollect ()
	collectgarbage("collect");
end

--  #######  ######## ##     ## ######## ########  
-- ##     ##    ##    ##     ## ##       ##     ## 
-- ##     ##    ##    ##     ## ##       ##     ## 
-- ##     ##    ##    ######### ######   ########  
-- ##     ##    ##    ##     ## ##       ##   ##   
-- ##     ##    ##    ##     ## ##       ##    ##  
--  #######     ##    ##     ## ######## ##     ##

--[[ 標籤 ]]
function this.tags (targetTags, ...)
	local tags = {...};
	for idx = 1, #tags do
		local eachTag = tags[idx];
		if (Util.contains(targetTags, eachTag) == false) then
			targetTags[#targetTags+1] = eachTag;
		end
	end
end

--[[ 是否符合標籤 ]]
function this.isTags (targetTags, tagOrTags)
	if (targetTags == nil) then return false end

	local tags = this.oneToArray(tagOrTags);

	for idx = 1, #tags do
		if (Util.contains(targetTags, tags[idx]) == false) then return false end
	end

	return true;
end

--[[ 三元運算 ]]
function this.isOr (isTrue, _true, _false)
	if (isTrue == true) then
		return _true;
	end
	return _false;
end

--[[ 序號轉換 ]]
function this.idxStarts0 (idxStarts1)
	return idxStarts1 - 1;
end

--[[ 序號轉換 ]]
function this.idxStarts1 (idxStarts0)
	return idxStarts0 + 1;
end

--[[ 深複製 ]]
function this.deepCopy (object)

	local searchTable = {};

	local func;
	func = function (object)

		if type(object) ~= "table" then
			return object;
		end

		local newTable = {};
		searchTable[object] = newTable;
		for k, v in pairs(object) do
			newTable[func(k)] = func(v);
		end

		return setmetatable(newTable, getmetatable(object));
	end

	return func(object);
end




return this;