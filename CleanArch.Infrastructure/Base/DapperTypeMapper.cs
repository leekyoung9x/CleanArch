using Dapper;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace CleanArch.Infrastructure.Base
{
    public static class DapperTypeMapper
    {
        private static readonly HashSet<Type> MappedTypes = new HashSet<Type>();
        private static readonly object Lock = new object();

        public static void SetupTypeMap<T>() where T : class
        {
            lock (Lock)
            {
                var type = typeof(T);
                if (MappedTypes.Contains(type))
                    return;

                var map = new CustomPropertyTypeMap(type, (t, columnName) =>
                {
                    var property = t.GetProperties().FirstOrDefault(prop =>
                    {
                        var columnAttr = prop.GetCustomAttribute<ColumnAttribute>();
                        if (columnAttr?.Name != null)
                        {
                            return columnAttr.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase);
                        }
                        return prop.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase);
                    });
                    return property;
                });

                SqlMapper.SetTypeMap(type, map);
                MappedTypes.Add(type);
            }
        }

        public static void SetupEntityMappings()
        {
            // Manually setup all entity mappings
            SetupTypeMap<CleanArch.Core.Entities.account>();
            SetupTypeMap<CleanArch.Core.Entities.Post>();
            SetupTypeMap<CleanArch.Core.Entities.rank>();
            SetupTypeMap<CleanArch.Core.Entities.transaction_banking>();
            SetupTypeMap<CleanArch.Core.Entities.transaction_card>();
            SetupTypeMap<CleanArch.Core.Entities.Contact>();
            
            // Reward system entities
            SetupTypeMap<CleanArch.Core.Entities.RewardPackage>();
            SetupTypeMap<CleanArch.Core.Entities.LeaderboardSeason>();
            SetupTypeMap<CleanArch.Core.Entities.RewardPackageContent>();
            SetupTypeMap<CleanArch.Core.Entities.MilestoneReward>();
            SetupTypeMap<CleanArch.Core.Entities.UserMilestoneClaim>();
            SetupTypeMap<CleanArch.Core.Entities.LeaderboardReward>();
            SetupTypeMap<CleanArch.Core.Entities.UserLeaderboardClaim>();
            
            // Item system entities
            SetupTypeMap<CleanArch.Core.Entities.ItemTemplate>();
            SetupTypeMap<CleanArch.Core.Entities.ItemOptionTemplate>();
        }
    }
}
