using Microsoft.Extensions.Options;
using System.Text.Json;
using azureTest.Config;
using azureTest.Models;
using azureTest.Services.Interfaces;

namespace azureTest.Services;

public class UsdaInfoService : IUsdaInfoService
{
    private Usda _usdaConfig;
    private readonly IHttpClientFactory _httpFactory;

    private string url = "";

    public UsdaInfoService(IOptions<Usda> opts,
        IHttpClientFactory httpFactory)
    {
        _usdaConfig = opts.Value;
        _httpFactory = httpFactory;
    }

    private string baseUrl = $"https://quickstats.nass.usda.gov/api/api_GET/?key=";

    public async Task<List<Datum>> GetUsdaDataObjectsRefactored(string metric, string commodity, string year, string short_desc)
    {
        url = BuildUsdaUrl(metric, commodity, year, short_desc);

        var dataObjects = new List<Datum>();
        var sortedDataObjects = new List<Datum>();

        var client = _httpFactory.CreateClient();
        var response = await client.GetAsync(url);
        var json = await response.Content.ReadAsStringAsync();
        var usdaResponse = JsonSerializer.Deserialize<UsdaInfo>(json);
        foreach (var item in usdaResponse.data)
        {
            if (item.agg_level_desc == "NATIONAL")
            {
                dataObjects.Add(new Datum
                {
                    prodn_practice_desc = item.prodn_practice_desc,
                    domain_desc = item.domain_desc,
                    county_name = item.county_name,
                    freq_desc = item.freq_desc,
                    begin_code = item.begin_code,
                    watershed_code = item.watershed_code,
                    end_code = item.end_code,
                    state_alpha = item.state_alpha,
                    agg_level_desc = item.agg_level_desc,
                    CV = item.CV,
                    state_ansi = item.state_ansi,
                    util_practice_desc = item.util_practice_desc,
                    region_desc = item.region_desc,
                    state_fips_code = item.state_fips_code,
                    county_code = item.county_code,
                    week_ending = item.week_ending,
                    year = item.year,
                    watershed_desc = item.watershed_desc,
                    unit_desc = item.unit_desc,
                    country_name = item.country_name,
                    domaincat_desc = item.domaincat_desc,
                    location_desc = item.location_desc,
                    zip_5 = item.zip_5,
                    group_desc = item.group_desc,
                    load_time = item.load_time,
                    Value = item.Value,
                    asd_desc = item.asd_desc,
                    county_ansi = item.county_ansi,
                    asd_code = item.asd_code,
                    commodity_desc = item.commodity_desc,
                    statisticcat_desc = item.statisticcat_desc,
                    congr_district_code = item.congr_district_code,
                    state_name = item.state_name,
                    reference_period_desc = item.reference_period_desc,
                    source_desc = item.source_desc,
                    class_desc = item.class_desc,
                    sector_desc = item.sector_desc,
                    country_code = item.country_code,
                    short_desc = item.short_desc
                }
                );
            }
        }

        // TODO: add filtering (maybe a switch case grabbed from front end...)

        sortedDataObjects = dataObjects.AsEnumerable()
            .Where(x => x.domain_desc == "TOTAL" || x.location_desc == "US TOTAL")
            .Where(x => x.source_desc != "CENSUS")
            .OrderByDescending(x => x.year)
            // TODO: consider filtering by reference_period_desc
            .ThenBy(x => x.load_time)
            .ToList();
        return sortedDataObjects;
    }
    
    public async Task<List<Datum>> GetUsdaDataObjectsStates(string metric, string commodity, string year, string short_desc)
    {
        //url = $"{baseUrl}{_usdaConfig.ApiKey}&statisticcat_desc=PROGRESS&year__LIKE={year}&commodity_desc=CORN&short_desc=CORN%20-%20PROGRESS,%20MEASURED%20IN%20PCT%20EMERGED";
        //url = $"{baseUrl}{_usdaConfig.ApiKey}&statisticcat_desc={metric}&year__LIKE={year}&commodity_desc={commodity}&short_desc={short_desc}";
        url = BuildUsdaUrl(metric, commodity, year, short_desc);

        var dataObjects = new List<Datum>();
        var sortedDataObjects = new List<Datum>();

        var client = _httpFactory.CreateClient();
        var response = await client.GetAsync(url);
        var json = await response.Content.ReadAsStringAsync();
        var usdaResponse = JsonSerializer.Deserialize<UsdaInfo>(json);
        foreach (var item in usdaResponse.data)
        {
            if (item.agg_level_desc != "NATIONAL")
            {
                dataObjects.Add(new Datum
                {
                    prodn_practice_desc = item.prodn_practice_desc,
                    domain_desc = item.domain_desc,
                    county_name = item.county_name,
                    freq_desc = item.freq_desc,
                    begin_code = item.begin_code,
                    watershed_code = item.watershed_code,
                    end_code = item.end_code,
                    state_alpha = item.state_alpha,
                    agg_level_desc = item.agg_level_desc,
                    CV = item.CV,
                    state_ansi = item.state_ansi,
                    util_practice_desc = item.util_practice_desc,
                    region_desc = item.region_desc,
                    state_fips_code = item.state_fips_code,
                    county_code = item.county_code,
                    week_ending = item.week_ending,
                    year = item.year,
                    watershed_desc = item.watershed_desc,
                    unit_desc = item.unit_desc,
                    country_name = item.country_name,
                    domaincat_desc = item.domaincat_desc,
                    location_desc = item.location_desc,
                    zip_5 = item.zip_5,
                    group_desc = item.group_desc,
                    load_time = item.load_time,
                    Value = item.Value,
                    asd_desc = item.asd_desc,
                    county_ansi = item.county_ansi,
                    asd_code = item.asd_code,
                    commodity_desc = item.commodity_desc,
                    statisticcat_desc = item.statisticcat_desc,
                    congr_district_code = item.congr_district_code,
                    state_name = item.state_name,
                    reference_period_desc = item.reference_period_desc,
                    source_desc = item.source_desc,
                    class_desc = item.class_desc,
                    sector_desc = item.sector_desc,
                    country_code = item.country_code,
                    short_desc = item.short_desc
                }
                );
            }
        }

        // TODO: add filtering (maybe a switch case grabbed from front end...)

        sortedDataObjects = dataObjects.AsEnumerable()
            //.Where(x => x.domain_desc == "TOTAL" || x.location_desc == "US TOTAL")
            .Where(x => x.source_desc != "CENSUS")
            .OrderByDescending(x => x.year)
            // TODO: consider filtering by reference_period_desc
            .ThenBy(x => x.load_time)
            .ToList();
        return sortedDataObjects;
    }

    public async Task<List<Datum>> GetUsdaDataObjectsRefactoredMultiYear(string metric, string commodity, string year, string short_desc)
    {
        url = BuildUsdaUrlMultiYear(metric, commodity, year, short_desc);

        var dataObjects = new List<Datum>();
        var sortedDataObjects = new List<Datum>();

        var client = _httpFactory.CreateClient();
        var response = await client.GetAsync(url);
        var json = await response.Content.ReadAsStringAsync();
        var usdaResponse = JsonSerializer.Deserialize<UsdaInfo>(json);
        foreach (var item in usdaResponse.data)
        {
            if (item.agg_level_desc == "NATIONAL")
            {
                dataObjects.Add(new Datum
                {
                    prodn_practice_desc = item.prodn_practice_desc,
                    domain_desc = item.domain_desc,
                    county_name = item.county_name,
                    freq_desc = item.freq_desc,
                    begin_code = item.begin_code,
                    watershed_code = item.watershed_code,
                    end_code = item.end_code,
                    state_alpha = item.state_alpha,
                    agg_level_desc = item.agg_level_desc,
                    CV = item.CV,
                    state_ansi = item.state_ansi,
                    util_practice_desc = item.util_practice_desc,
                    region_desc = item.region_desc,
                    state_fips_code = item.state_fips_code,
                    county_code = item.county_code,
                    week_ending = item.week_ending,
                    year = item.year,
                    watershed_desc = item.watershed_desc,
                    unit_desc = item.unit_desc,
                    country_name = item.country_name,
                    domaincat_desc = item.domaincat_desc,
                    location_desc = item.location_desc,
                    zip_5 = item.zip_5,
                    group_desc = item.group_desc,
                    load_time = item.load_time,
                    Value = item.Value,
                    asd_desc = item.asd_desc,
                    county_ansi = item.county_ansi,
                    asd_code = item.asd_code,
                    commodity_desc = item.commodity_desc,
                    statisticcat_desc = item.statisticcat_desc,
                    congr_district_code = item.congr_district_code,
                    state_name = item.state_name,
                    reference_period_desc = item.reference_period_desc,
                    source_desc = item.source_desc,
                    class_desc = item.class_desc,
                    sector_desc = item.sector_desc,
                    country_code = item.country_code,
                    short_desc = item.short_desc
                }
                );
            }
        }

        // TODO: add filtering (maybe a switch case grabbed from front end...)

        sortedDataObjects = dataObjects.AsEnumerable()
            .Where(x => x.domain_desc == "TOTAL" || x.location_desc == "US TOTAL")
            .OrderByDescending(x => x.year)
            // TODO: consider filtering by reference_period_desc
            .ThenByDescending(x => x.load_time)
            .ToList();
        return sortedDataObjects;
    }


    private string BuildUsdaUrl(string metric, string commodity, string year, string short_desc)
    {
        _usdaConfig.ApiKey = "C2ADF26B-BD8D-328A-968F-2F175A287144";
        switch (metric)
        {
            case "AREA PLANTED":
                short_desc = $"{commodity} - ACRES PLANTED";
                url = $"{baseUrl}{_usdaConfig.ApiKey}&statisticcat_desc={metric}&unit_desc=ACRES&year__LIKE={year}&commodity_desc={commodity}";
                break;
            case "AREA HARVESTED":
                url = $"{baseUrl}{_usdaConfig.ApiKey}&commodity_desc={commodity}&statistic_cat_desc={metric}&unit_desc=ACRES&year__LIKE={year}";
                break;
            case "CONDITION":
                url = $"{baseUrl}{_usdaConfig.ApiKey}&year__LIKE={year}&commodity_desc={commodity}&short_desc={short_desc}&statisticcat_desc={metric}";
                break;
            case "ETHANOL USAGE":
                short_desc = $"{commodity}, FOR FUEL ALCOHOL - USAGE, MEASURED IN BU";
                url = $"{baseUrl}{_usdaConfig.ApiKey}&year__LIKE={year}&short_desc={short_desc}";
                break;
            case "PRODUCTION":
                if (commodity == "CORN")
                {
                    short_desc = "CORN, GRAIN - PRODUCTION, MEASURED IN BU";
                    url = $"{baseUrl}{_usdaConfig.ApiKey}&year__LIKE={year}&commodity={commodity}&metric={metric}&short_desc={short_desc}";
                }
                else
                {
                    short_desc = "SOYBEANS - PRODUCTION, MEASURED IN BU";
                    url = $"{baseUrl}{_usdaConfig.ApiKey}&year__LIKE={year}&commodity={commodity}&metric={metric}&short_desc={short_desc}";
                }
                break;
            case "PROGRESS":
                short_desc = $"{commodity} - PROGRESS, MEASURED IN PCT EMERGED";
                url = $"{baseUrl}{_usdaConfig.ApiKey}&year__LIKE={year}&short_desc={short_desc}";
                break;
            case "RESIDUAL USAGE":
                short_desc = "CORN, FOR OTHER PRODUCTS (EXCL ALCOHOL) - USAGE, MEASURED IN BU";
                url = $"{baseUrl}{_usdaConfig.ApiKey}&year__LIKE={year}&short_desc={short_desc}";
                break;
            case "STOCKS":
                if (commodity == "CORN")
                {
                    short_desc = "CORN, GRAIN - PRODUCTION, MEASURED IN BU";
                    url = $"{baseUrl}{_usdaConfig.ApiKey}&year__LIKE={year}&short_desc={short_desc}";
                }
                else
                {
                    short_desc = "SOYBEANS - STOCKS, MEASURED IN BU";
                    url = $"{baseUrl}{_usdaConfig.ApiKey}&year__LIKE={year}&short_desc={short_desc}";
                }
                break;
            case "PROGRESS, 5 YEAR AVG, MEASURED IN PCT PLANTED":
                short_desc = $"{commodity} - PROGRESS, 5 YEAR AVG, MEASURED IN PCT PLANTED";
                url = $"{baseUrl}{_usdaConfig.ApiKey}&year__LIKE={year}&short_desc={short_desc}";
                break;
            case "CONDITION, 5 YEAR AVG, MEASURED IN PCT EXCELLENT":
                short_desc = $"{commodity} - CONDITION, 5 YEAR AVG, MEASURED IN PCT EXCELLENT";
                url = $"{baseUrl}{_usdaConfig.ApiKey}&year__LIKE={year}&metric={metric}&short_desc={short_desc}";
                break;
            default:
                break;
        }

        return url;
    }

    private string BuildUsdaUrlMultiYear(string metric, string commodity, string year, string short_desc)
    {
        _usdaConfig.ApiKey = "C2ADF26B-BD8D-328A-968F-2F175A287144";

        switch (metric)
        {
            case "AREA PLANTED":
                if (year.Contains(','))
                {
                    //year.Substring(4, 4).
                }
                short_desc = $"{commodity} - ACRES PLANTED";
                url = $"{baseUrl}{_usdaConfig.ApiKey}&statisticcat_desc={metric}&unit_desc=ACRES&year__LIKE={year}&commodity_desc={commodity}";
                break;
            case "AREA HARVESTED":
                url = $"{baseUrl}{_usdaConfig.ApiKey}&commodity_desc={commodity}&statistic_cat_desc={metric}&unit_desc=ACRES&year__LIKE={year}";
                break;
            case "CONDITION":
                url = $"{baseUrl}{_usdaConfig.ApiKey}&year__LIKE={year}&commodity_desc={commodity}&short_desc={short_desc}&statisticcat_desc={metric}";
                break;
            case "ETHANOL USAGE":
                short_desc = $"{commodity}, FOR FUEL ALCOHOL - USAGE, MEASURED IN BU";
                url = $"{baseUrl}{_usdaConfig.ApiKey}&year__LIKE={year}&short_desc={short_desc}";
                break;
            case "PRODUCTION":
                if (commodity == "CORN")
                {
                    short_desc = "CORN, GRAIN - PRODUCTION, MEASURED IN BU";
                    url = $"{baseUrl}{_usdaConfig.ApiKey}&year__LIKE={year}&commodity={commodity}&metric={metric}&short_desc={short_desc}";
                }
                else
                {
                    short_desc = "SOYBEANS - PRODUCTION, MEASURED IN BU";
                    url = $"{baseUrl}{_usdaConfig.ApiKey}&year__LIKE={year}&commodity={commodity}&metric={metric}&short_desc={short_desc}";
                }
                break;
            case "PROGRESS":
                short_desc = $"{commodity} - PROGRESS, MEASURED IN PCT EMERGED";
                url = $"{baseUrl}{_usdaConfig.ApiKey}&year__LIKE={year}&short_desc={short_desc}";
                break;
            case "RESIDUAL USAGE":
                short_desc = "CORN, FOR OTHER PRODUCTS (EXCL ALCOHOL) - USAGE, MEASURED IN BU";
                url = $"{baseUrl}{_usdaConfig.ApiKey}&year__LIKEe={year}&short_desc={short_desc}";
                break;
            case "STOCKS":
                if (commodity == "CORN")
                {
                    short_desc = "CORN, GRAIN - PRODUCTION, MEASURED IN BU";
                    url = $"{baseUrl}{_usdaConfig.ApiKey}&year__LIKE={year}&short_desc={short_desc}";
                }
                else
                {
                    short_desc = "SOYBEANS - STOCKS, MEASURED IN BU";
                    url = $"{baseUrl}{_usdaConfig.ApiKey}&year__LIKE={year}&short_desc={short_desc}";
                }
                break;
            default:
                break;
        }

        return url;
    }


    public List<Datum> GetUsdaDataObjectsByState(string metric, string commodity, string year, string short_desc, string stateAlpha)
    {
        switch (metric)
        {
            case "AREA PLANTED":
                short_desc = $"{commodity} - ACRES PLANTED";
                url = $"{baseUrl}{_usdaConfig.ApiKey}&statisticcat_desc={metric}&unit_desc=ACRES&year__GE={year}&commodity_desc={commodity}";
                break;
            case "AREA HARVESTED":
                url = $"{baseUrl}{_usdaConfig.ApiKey}&commodity_desc={commodity}&statistic_cat_desc={metric}&unit_desc=ACRES&year__GE={year}";
                break;
            case "CONDITION":
                url = $"{baseUrl}{_usdaConfig.ApiKey}&year__GE={year}&short_desc={commodity}{short_desc}&commodity_desc={commodity}&statisticcat_desc={metric}";
                break;
            case "ETHANOL USAGE":
                short_desc = $"{commodity}, FOR FUEL ALCOHOL - USAGE, MEASURED IN BU";
                url = $"{baseUrl}{_usdaConfig.ApiKey}&year__GE={year}&short_desc={short_desc}";
                break;
            case "PRODUCTION":
                if (commodity == "CORN")
                {
                    short_desc = "CORN, GRAIN - PRODUCTION, MEASURED IN BU";
                    url = $"{baseUrl}{_usdaConfig.ApiKey}&year__GE={year}&commodity={commodity}&metric={metric}";
                }
                else
                {
                    short_desc = "SOYBEANS - PRODUCTION, MEASURED IN BU";
                    url = $"{baseUrl}{_usdaConfig.ApiKey}&year__GE={year}&commodity={commodity}&metric={metric}";
                }
                break;
            case "PROGRESS":
                short_desc = $"{commodity} - PROGRESS, MEASURED IN PCT EMERGED";
                url = $"{baseUrl}{_usdaConfig.ApiKey}&year__GE={year}&short_desc={short_desc}";
                break;
            case "RESIDUAL USAGE":
                short_desc = "CORN, FOR OTHER PRODUCTS (EXCL ALCOHOL) - USAGE, MEASURED IN BU";
                url = $"{baseUrl}{_usdaConfig.ApiKey}&year__GE={year}&short_desc={short_desc}";
                break;
            case "STOCKS":
                url = $"{baseUrl}{_usdaConfig.ApiKey}&year__GE={year}&short_desc={short_desc}";
                break;
            default:
                break;
        }

        var dataObjects = new List<Datum>();
        var filteredDataObjects = new List<Datum>();

        var client = _httpFactory.CreateClient();

        var response = client.GetAsync(url).Result;
            var json = response.Content.ReadAsStringAsync().Result;
            var usdaResponse = JsonSerializer.Deserialize<UsdaInfo>(json);
            foreach (var item in usdaResponse.data)
            {
                dataObjects.Add(new Datum
                {
                    prodn_practice_desc = item.prodn_practice_desc,
                    domain_desc = item.domain_desc,
                    county_name = item.county_name,
                    freq_desc = item.freq_desc,
                    begin_code = item.begin_code,
                    watershed_code = item.watershed_code,
                    end_code = item.end_code,
                    state_alpha = item.state_alpha,
                    agg_level_desc = item.agg_level_desc,
                    CV = item.CV,
                    state_ansi = item.state_ansi,
                    util_practice_desc = item.util_practice_desc,
                    region_desc = item.region_desc,
                    state_fips_code = item.state_fips_code,
                    county_code = item.county_code,
                    week_ending = item.week_ending,
                    year = item.year,
                    watershed_desc = item.watershed_desc,
                    unit_desc = item.unit_desc,
                    country_name = item.country_name,
                    domaincat_desc = item.domaincat_desc,
                    location_desc = item.location_desc,
                    zip_5 = item.zip_5,
                    group_desc = item.group_desc,
                    load_time = item.load_time,
                    Value = item.Value,
                    asd_desc = item.asd_desc,
                    county_ansi = item.county_ansi,
                    asd_code = item.asd_code,
                    commodity_desc = item.commodity_desc,
                    statisticcat_desc = item.statisticcat_desc,
                    congr_district_code = item.congr_district_code,
                    state_name = item.state_name,
                    reference_period_desc = item.reference_period_desc,
                    source_desc = item.source_desc,
                    class_desc = item.class_desc,
                    sector_desc = item.sector_desc,
                    country_code = item.country_code,
                    short_desc = item.short_desc
                }
                );
            }

        foreach (var item in dataObjects)
        {
            if (item.state_alpha.Contains(stateAlpha))
            {
                filteredDataObjects.Add(item);
            }
        }
        //return (List<Datum>)dataObjects.OrderBy(x => x.load_time);
        //return dataObjects;
        filteredDataObjects.OrderByDescending(x => x.county_name).ToList();
        return filteredDataObjects;
    }

    public List<Datum> GetUsdaDataObjectsNoParams()
    {
        string url = $"{baseUrl}key=C2ADF26B-BD8D-328A-968F-2F175A287144&statisticcat_desc=AREA PLANTED&unit_desc=ACRES&year__GE=2020&commodity_desc=CORN";
        var dataObjects = new List<Datum>();

        var client = _httpFactory.CreateClient();

        var response = client.GetAsync(url).Result;
        var json = response.Content.ReadAsStringAsync().Result;
        var usdaResponse = JsonSerializer.Deserialize<UsdaInfo>(json);
        foreach (var item in usdaResponse.data)
        {
            dataObjects.Add(new Datum
            {
                prodn_practice_desc = item.prodn_practice_desc,
                domain_desc = item.domain_desc,
                county_name = item.county_name,
                freq_desc = item.freq_desc,
                begin_code = item.begin_code,
                watershed_code = item.watershed_code,
                end_code = item.end_code,
                state_alpha = item.state_alpha,
                agg_level_desc = item.agg_level_desc,
                CV = item.CV,
                state_ansi = item.state_ansi,
                util_practice_desc = item.util_practice_desc,
                region_desc = item.region_desc,
                state_fips_code = item.state_fips_code,
                county_code = item.county_code,
                week_ending = item.week_ending,
                year = item.year,
                watershed_desc = item.watershed_desc,
                unit_desc = item.unit_desc,
                country_name = item.country_name,
                domaincat_desc = item.domaincat_desc,
                location_desc = item.location_desc,
                zip_5 = item.zip_5,
                group_desc = item.group_desc,
                load_time = item.load_time,
                Value = item.Value,
                asd_desc = item.asd_desc,
                county_ansi = item.county_ansi,
                asd_code = item.asd_code,
                commodity_desc = item.commodity_desc,
                statisticcat_desc = item.statisticcat_desc,
                congr_district_code = item.congr_district_code,
                state_name = item.state_name,
                reference_period_desc = item.reference_period_desc,
                source_desc = item.source_desc,
                class_desc = item.class_desc,
                sector_desc = item.sector_desc,
                country_code = item.country_code,
                short_desc = item.short_desc
            }
            );
        }


        return dataObjects;
    }

    //to be deleted
    public List<Datum> GetUsdaDataObjectsOld(string metric, string commodity, string year)
    {
        string url = $"{baseUrl}{_usdaConfig.ApiKey}&statisticcat_desc={metric}&unit_desc=ACRES&year__GE={year}&commodity_desc={commodity}";
        var dataObjects = new List<Datum>();

        var client = _httpFactory.CreateClient();

        var response = client.GetAsync(url).Result;
            var json = response.Content.ReadAsStringAsync().Result;
            var usdaResponse = JsonSerializer.Deserialize<UsdaInfo>(json);
            foreach (var item in usdaResponse.data)
            {
                dataObjects.Add(new Datum
                {
                    prodn_practice_desc = item.prodn_practice_desc,
                    domain_desc = item.domain_desc,
                    county_name = item.county_name,
                    freq_desc = item.freq_desc,
                    begin_code = item.begin_code,
                    watershed_code = item.watershed_code,
                    end_code = item.end_code,
                    state_alpha = item.state_alpha,
                    agg_level_desc = item.agg_level_desc,
                    CV = item.CV,
                    state_ansi = item.state_ansi,
                    util_practice_desc = item.util_practice_desc,
                    region_desc = item.region_desc,
                    state_fips_code = item.state_fips_code,
                    county_code = item.county_code,
                    week_ending = item.week_ending,
                    year = item.year,
                    watershed_desc = item.watershed_desc,
                    unit_desc = item.unit_desc,
                    country_name = item.country_name,
                    domaincat_desc = item.domaincat_desc,
                    location_desc = item.location_desc,
                    zip_5 = item.zip_5,
                    group_desc = item.group_desc,
                    load_time = item.load_time,
                    Value = item.Value,
                    asd_desc = item.asd_desc,
                    county_ansi = item.county_ansi,
                    asd_code = item.asd_code,
                    commodity_desc = item.commodity_desc,
                    statisticcat_desc = item.statisticcat_desc,
                    congr_district_code = item.congr_district_code,
                    state_name = item.state_name,
                    reference_period_desc = item.reference_period_desc,
                    source_desc = item.source_desc,
                    class_desc = item.class_desc,
                    sector_desc = item.sector_desc,
                    country_code = item.country_code,
                    short_desc = item.short_desc
                }
                );
            }

        return dataObjects;
    }

    public List<Datum> GetUsdaDataObjects(string metric, string commodity, string year, string short_desc)
    {
        switch (metric)
        {
            case "AREA PLANTED":
                short_desc = $"{commodity} - ACRES PLANTED";
                url = $"{baseUrl}{_usdaConfig.ApiKey}&statisticcat_desc={metric}&unit_desc=ACRES&year__GE={year}&commodity_desc={commodity}";
                break;
            case "AREA HARVESTED":
                url = $"{baseUrl}{_usdaConfig.ApiKey}&commodity_desc={commodity}&statistic_cat_desc={metric}&unit_desc=ACRES&year__GE={year}";
                break;
            case "CONDITION":
                url = $"{baseUrl}{_usdaConfig.ApiKey}&year__GE={year}&commodity_desc={commodity}&short_desc={short_desc}&statisticcat_desc={metric}";
                break;
            case "ETHANOL USAGE":
                short_desc = $"{commodity}, FOR FUEL ALCOHOL - USAGE, MEASURED IN BU";
                url = $"{baseUrl}{_usdaConfig.ApiKey}&year__GE={year}&short_desc={short_desc}";
                break;
            case "PRODUCTION":
                if (commodity == "CORN")
                {
                    short_desc = "CORN, GRAIN - PRODUCTION, MEASURED IN BU";
                    url = $"{baseUrl}{_usdaConfig.ApiKey}&year__GE={year}&commodity={commodity}&metric={metric}&short_desc={short_desc}";
                }
                else
                {
                    short_desc = "SOYBEANS - PRODUCTION, MEASURED IN BU";
                    url = $"{baseUrl}{_usdaConfig.ApiKey}&year__GE={year}&commodity={commodity}&metric={metric}&short_desc={short_desc}";
                }
                break;
            case "PROGRESS":
                short_desc = $"{commodity} - PROGRESS, MEASURED IN PCT EMERGED";
                url = $"{baseUrl}{_usdaConfig.ApiKey}&year__GE={year}&short_desc={short_desc}";
                break;
            case "RESIDUAL USAGE":
                short_desc = "CORN, FOR OTHER PRODUCTS (EXCL ALCOHOL) - USAGE, MEASURED IN BU";
                url = $"{baseUrl}{_usdaConfig.ApiKey}&year__GE={year}&short_desc={short_desc}";
                break;
            case "STOCKS":
                if (commodity == "CORN")
                {
                    short_desc = "CORN, GRAIN - PRODUCTION, MEASURED IN BU";
                    url = $"{baseUrl}{_usdaConfig.ApiKey}&year__GE={year}&short_desc={short_desc}";
                }
                else
                {
                    short_desc = "SOYBEANS - STOCKS, MEASURED IN BU";
                    url = $"{baseUrl}{_usdaConfig.ApiKey}&year__GE={year}&short_desc={short_desc}";
                }
                break;
            default:
                break;
        }

        var dataObjects = new List<Datum>();
        var sortedDataObjects = new List<Datum>();

        var client = _httpFactory.CreateClient();

        var response = client.GetAsync(url).Result;
            var json = response.Content.ReadAsStringAsync().Result;
            var usdaResponse = JsonSerializer.Deserialize<UsdaInfo>(json);
            foreach (var item in usdaResponse.data)
            {
                if (item.agg_level_desc == "NATIONAL")
                {
                    dataObjects.Add(new Datum
                    {
                        prodn_practice_desc = item.prodn_practice_desc,
                        domain_desc = item.domain_desc,
                        county_name = item.county_name,
                        freq_desc = item.freq_desc,
                        begin_code = item.begin_code,
                        watershed_code = item.watershed_code,
                        end_code = item.end_code,
                        state_alpha = item.state_alpha,
                        agg_level_desc = item.agg_level_desc,
                        CV = item.CV,
                        state_ansi = item.state_ansi,
                        util_practice_desc = item.util_practice_desc,
                        region_desc = item.region_desc,
                        state_fips_code = item.state_fips_code,
                        county_code = item.county_code,
                        week_ending = item.week_ending,
                        year = item.year,
                        watershed_desc = item.watershed_desc,
                        unit_desc = item.unit_desc,
                        country_name = item.country_name,
                        domaincat_desc = item.domaincat_desc,
                        location_desc = item.location_desc,
                        zip_5 = item.zip_5,
                        group_desc = item.group_desc,
                        load_time = item.load_time,
                        Value = item.Value,
                        asd_desc = item.asd_desc,
                        county_ansi = item.county_ansi,
                        asd_code = item.asd_code,
                        commodity_desc = item.commodity_desc,
                        statisticcat_desc = item.statisticcat_desc,
                        congr_district_code = item.congr_district_code,
                        state_name = item.state_name,
                        reference_period_desc = item.reference_period_desc,
                        source_desc = item.source_desc,
                        class_desc = item.class_desc,
                        sector_desc = item.sector_desc,
                        country_code = item.country_code,
                        short_desc = item.short_desc
                    }
                    );
                }
            }
        

        //foreach (var item in dataObjects)
        //{
        //    if (item.reference_period_desc.Contains("ACREAGE"));
        //    filteredDataObjects.Add(item);
        //}
        sortedDataObjects = dataObjects.AsEnumerable()
            .Where(x => x.domain_desc == "TOTAL" || x.location_desc == "US TOTAL")
            .OrderByDescending(x => x.year)
            .ThenByDescending(x => x.load_time)
            .ToList();
        //return dataObjects;
        return sortedDataObjects;
    }

    //public async Task<IActionResult> GetUsdaData(string Metrick, string Commodity, string Year)
    //public async List<string> GetUsdaData(string Metrick, string Commodity, string Year)
    //{
    //    var apiUrl = $"https://quickstats.nass.usda.gov/api/api_GET/?key={_usdaConfig.ApiKey}&statisticcat_desc={Metric}&unit_desc=ACRES&year__GE={Year}&commodity_desc={Commodity}";

    //    HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

    //    if (response.IsSuccessStatusCode)
    //    {
    //        string data = await response.Content.ReadAsStringAsync();
    //        //string data = await response.Content.ReadFromJsonAsync<string>();
    //        var json = response.Content.ReadAsStringAsync().Result;

    //        //dataSet = JsonSerializer.Deserialize<UsdaInfo>(json);

    //        return data;
    //    }
    //    else
    //    {
    //        return (int)response.StatusCode;
    //    }
    //}



    //public List<UsdaInfo> GetUsdaInfos()
    //{
    //    //string url = $"https://api.openweathermap.org/data/2.5/forecast?q={location}&appid={_openWeatherConfig.ApiKey}&units={unit}";
    //    string url = $"https://quickstats.nass.usda.gov/api/api_GET/?key={_usdaConfig.ApiKey}&statisticcat_desc=AREA PLANTED&unit_desc=ACRES&year__GE=2020/";
    //    var usdaInfos = new List<UsdaInfo>();

    //    using (HttpClient client = new HttpClient())
    //    {
    //        var response = client.GetAsync(url).Result;
    //        var json = response.Content.ReadAsStringAsync().Result;
    //        var usdaInfoResponse = JsonSerializer.Deserialize<UsdaInfoResponse>(json);
    //        foreach (var usdaInfo in usdaInfoResponse.UsdaInfos)
    //        {
    //            usdaInfos.Add(new UsdaInfo
    //            {
    //                data =
    //                [
    //                    new Datum
    //                    {
    //                        state_ansi = usdaInfo.data.FirstOrDefault().state_ansi,
    //                        congr_district_code = usdaInfo.data.FirstOrDefault().congr_district_code
    //                    }
    //                ]
    //            });;
    //        }
    //    }

    //    return usdaInfos;
    //}
}
