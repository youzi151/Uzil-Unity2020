local this = {};


--[[ 取得 實例 ]] -- table
function this.inst (key)
	local inst = {};

	inst.key = key;

	--[[ 取得 池隨機 ]] -- table
	inst.getPoolRandom = function (id)

		local poolRandom = {};

		poolRandom.id = id;

		--[[ 設置 池隨機 ]] -- self
		poolRandom.set = function (data)
			this.setPoolRandom(inst.key, poolRandom.id, data);
			return poolRandom;
		end
		
		--[[ 設置 池隨機 種子 ]] -- self
		poolRandom.seed = function (seed)
			this.setPoolRandomSeed(inst.key, poolRandom.id, seed);
			return poolRandom;
		end
		
		--[[ 設置 池隨機 範圍 ]] -- self
		poolRandom.range = function (min, max)
			this.setPoolRandomRange(inst.key, poolRandom.id, min, max);
			return poolRandom;
		end

		--[[ 設置 池隨機 比率 ]] -- self
		poolRandom.rate = function (num, rate)
			this.setPoolRandomRate(inst.key, poolRandom.id, num, rate);
			return poolRandom;
		end

		--[[ 取得 池隨機 隨機值 ]] -- number
		poolRandom.getNum = function ()
			return this.getPoolRandomNum(inst.key, poolRandom.id);
		end
		--[[ 取得 池隨機 隨機整數 ]] -- number
		poolRandom.getInt = function ()
			return this.getPoolRandomInt(inst.key, poolRandom.id);
		end

		--[[ 重置 ]]
		poolRandom.reset = function ()
			this.resetPoolRandom(inst.key, poolRandom.id);
			return poolRandom;
		end

		return poolRandom;
	end

	--[[ 取得 加權隨機 ]]
	inst.getRateRandom = function ()
		local rateRandom = {};

		--[[ 值:權重 ]]
		rateRandom._value2pairIdx = {};
		rateRandom._pairs = {};

		--[[ 設置 權重 ]]
		rateRandom.rate = function (rate, value)
			local idx;
			if (Util.containsKey(rateRandom._value2pairIdx, value)) then
				idx = rateRandom._value2pairIdx[value];
			else
				idx = #rateRandom._pairs+1;
				rateRandom._value2pairIdx[value] = idx;
			end
			rateRandom._pairs[idx] = {rate, value};
			return rateRandom;
		end

		--[[ 取得 隨機值 ]]
		rateRandom.get = function (seed)
			return this.getRateRandomValue(rateRandom, seed);
		end

		return rateRandom;
	end

	return inst;
end


--[[ 設置 池隨機 ]] -- void
function this.setPoolRandom (instKey, id, data)
	local data_str = Json.encode(data);
	UZAPI.RNG.SetPoolRandom(instKey, id, data_str);
end

--[[ 設置 池隨機 種子 ]] -- void
function this.setPoolRandomSeed (instKey, id, seed)
	UZAPI.RNG.SetPoolRandomSeed(instKey, id, seed);
end

--[[ 設置 池隨機 範圍 ]] -- void
function this.setPoolRandomRange (instKey, id, min, max)
	UZAPI.RNG.SetPoolRandomRange(instKey, id, min, max);
end

--[[ 設置 池隨機 比率 ]] -- void
function this.setPoolRandomRate (instKey, id, num, rate)
	UZAPI.RNG.SetPoolRandomRate(instKey, id, num, rate);
end

--[[ 取得 池隨機 隨機值 ]] -- number
function this.getPoolRandomNum (instKey, id)
	return UZAPI.RNG.GetPoolRandomNum(instKey, id);
end

--[[ 取得 池隨機 隨機整數 ]] -- number
function this.getPoolRandomInt (instKey, id)
	return UZAPI.RNG.GetPoolRandomInt(instKey, id);
end

--[[ 重置 ]]
function this.resetPoolRandom (instKey, id)
	UZAPI.RNG.ResetPoolRandom(instKey, id);
end

--[[ 取得 加權隨機值 ]]
function this.getRateRandomValue (rateRandom, seed)
	local rates = {};
	local values = {};
	for idx = 1, #rateRandom._pairs do
		local pair = rateRandom._pairs[idx];
		rates[#rates+1] = pair[1];
		values[#values+1] = pair[2];
	end
	local idx = UZAPI.RNG.GetRateRandom(Json.encode(rates), tostring(seed)) + 1;
	return values[idx];
end



return this;