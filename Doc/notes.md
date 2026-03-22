# Problems that I see with our current design: 
1. Asset manager only loads production units that are specified in the list on Asset manager initialization. 
It is bad, because then it will be impossible to choose diffrent units in the app's interface. 

2. We need a method that goes through the whole HourlyData list and calls DistributeHeatLoad(), returning final resultData. 

3. Gotta move all the things related to SQL to a separate branch and remove them from main. 

4. Do we ever need to save a single resultData actually, or should it becomne private and only SaveMany be exposed as public?  

5. Remember YAGNI principles when designing the app. We seem to create a variety of methods, but not all of them might actually necessary.

6. Optimizer should not depend on a ResultManager or even IResultManager I think, because then it is hard to make it reusable for anything else. 

7. Perhaps, we shouldn't even have AssetManager injected into the optimizer. It creates unnecessary coupling, while it doesn't really provide any benefits. Why wouldn't I simply call a method in the Opmizer in a form of 

```cs
var units = assetManager.getProductionUnits(["GB1", "GB2", "GB3", "OB1"]);
var data = assetManager.getHourlyData();
var resultData = optimizer.process(data, units);
resultDataManager.SetResultData(resultData);
// line above would trigger UI update, because resultData in the resultDataManager is an ObservableProperty.
```