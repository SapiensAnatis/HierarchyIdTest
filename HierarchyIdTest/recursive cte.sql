WITH service_hierarchy(Id, Parent, HierarchyLevel)
         AS (SELECT Id, Parent, 1 as [c1]
FROM Service
WHERE Id = 14
UNION ALL
SELECT s.Id,
       s.Parent,
    [sh].[HierarchyLevel] + 1 as [c1]
FROM Service s
    INNER JOIN service_hierarchy sh on sh.Parent = s.Id)
SELECT  Id, Parent, HierarchyLevel, ParameterName, ParameterValue
FROM service_hierarchy
         LEFT JOIN ServiceParameter sp on sp.ServiceId = service_hierarchy.Id
