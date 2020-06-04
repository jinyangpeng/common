-- 查询表结构
set @rank :=0;
select @rank := @rank + 1 as rowId, column_name, column_type, column_comment, is_nullable, column_default, ' ' as '数值含义', ' ' as '是否在用'
from information_schema.columns 
where table_name='tableName' and table_schema='tableSchema';


select GROUP_CONCAT(column_name)
from information_schema.columns 
where table_name='tableName' and table_schema='tableSchema';




