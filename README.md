# Urbetrack
NET Exercise Practice

Hola! Este esta es mi resolución del Challenge de NET.

Vía Google Drive he compartido la resolución con la funcionalidad mínima requerida del ejercicio (la misma se encuentra en "master"). Además, en la branch "feature-1" he creado un repositorio genérico aplicando Repository Pattern. Mi idea en principio era crear un contexto, pero debido a las contraints planteadas (la de no agregar dependencias externas al proyecto, por ejemplo) no podría haber usado EF o alguna dependencia que necesitara. Por lo que decidí simular o "mockear" un contexto de Listas en lugar de un DbContext con sus respectivos DbSets, con el fin de darle escalabilidad al proyecto.
