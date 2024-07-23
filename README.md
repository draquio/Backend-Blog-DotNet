# Backend Blog + JWT + SQLServer + EmailService

# Características del sistema.
- Implementación de la arquitectura en capas.
- Patrón de capa Servicio para la lógica de negocio.
- Patrón Repositorio para el manejo de la base de datos.
- Patrón DTO para crear, listar y actualizar datos (cada modelo tiene sus DTOs).
- AutoMapper para la mapear de manera más eficiente los DTOs a modelos y separar la lógica de mapeo del servicio.
- Inyección de dependencia para servicios, repositorios, mappers, etc.
- Validación de modelos DTOs con FluentValidation.
- Validaciones genéricas como Id, Email, etc.
- Middleware para gestionar todas las excepciones (deja limpio el controlador).
- Subida de imágenes al directorio wwwroot.
- Envío de correos electrónicos para Auth.
- Model First (creación, migración y versionamiento de la base de datos a partir del modelo).
- Interfaces de Servicio y repositorio.
- Cors activada para patición desde cualquier url.
![](https://i.ibb.co/dgk3VPV/image.png)

# Auth para autenticación y autorización
El sistema de Auth que cuenta de controlador, servicio y repositorio permite gestionar permisos y roles de acceso a lo endpoints contando con:
- Login.- Genera AccessToken y RefreshToken.
- RefreshToken.- Permite mantener la sesión vigente con el refresh token.
- Register.- Registro de usuarios con validación de correo para activar la cuenta y poder acceder.
- Verify.- Endpoint de verificación del correo electrónico.
- Request Password Reset.- Realiza una petición con el correo para resetear contraseña, se envía un correo de verificación.
- Reset Password.- Endpoint de verificación para el reseteo de password, genera una nueva contraseña aleatoria y la envía por correo.
![](https://i.ibb.co/vLNjhxf/image.png)

# Post
El sistema permite una gestión completa de blogpost solicitando permisos de Editar y/o Administrador.
- Crear post.- Permite crear post con tags, categorías, miniatura, fecha de creación y actualización, estado y demás. (Administrador y Editor)
    - Título
    - Contenido
    - UserId (usuario que realizó la publicación)
    - ImageId (miniatura de la publicación, opcional)
    - Fecha de creación
    - Fecha de actualización (se actualiza cada que hacen un update del post)
    - Published (estado para ver si fue publicado o esta sin publicar)
    - Tags (lista de string para tags)
- Listar post con paginación y, filtro de publicados y no publicados. (IsPublished = true / false / null para todos)
- Obtener post por Id.
- Editar post por Id. (Administrador y Editor)
- Eliminar post por Id. (Administrador y Editor)
- Filtrar post.- Permite filtrar los post por diferentes filtros como:
    - Filtro por categoría (id)
    - Filtro por usuario (id)
    - Filtro por fecha de creación (YYYY-MM-DD)
    - Filtro por tag
- Buscar post por término.- Busca post por termino o palabras claves que se encuentren en el título o contenido del post.
![](https://i.ibb.co/f1JX31V/image.png)

# Usuarios
Gestión de usuarios con Jwt y diferentes roles y permisos:
- Crear usuario.- Permite crear nuevos usuarios directamente con la cuenta activa (solo Administrador) (IsActive = true)
- Listar usuarios con paginación.
- Obtener usuario por Id.
- Editar usuario por Id (solo Administrador).
- Eliminar usuario por Id (solo Administrador).
- Modificar contraseña.- Permite actualizar la contraseña del usuario (requiere estar logeado).
![](https://i.ibb.co/87F5j3q/image.png)

# Imagenes
Permite subir imágenes a la dirección wwwroot aceptando solo formatos de imagenes
- Subir imágenes (Administrador y Editor)
- Listar imagenes con paginación
- Obtener imagenes por id
- Editar imágenes por Id (Administrador y Editor)
- Eliminar imágenes por Id (Administrador y Editor)
![](https://i.ibb.co/Tm4QXgc/img.png)

# Category
La sección de category permite gestionar las categorías las cuales pueden relacionarse a los post de muchos a muchos, esta permite:
- Crear categrías (solo Administrador)
- Listar categorías con paginación
- Obtener categoría por Id
- Editar categoría por Id (solo Administrador)
- Borrado lógico de categoría por Id (solo Administrador) (IsDelete = true)
![](https://i.ibb.co/Bn2KKkf/image.png)

# Comments
La sección de comments permite a los visitantes del blog comentar publicaciones:
- Crear comentarios.- Endpoint público.
- Listar comentarios aprobados y públicos por PostId.
- Listar todos los comentarios con filtro para todos, aprobados y sin aprobar por PostId (requiere estar logeado).
- Editar comentario por Id (Administrador y Editor)
- Eliminar comentario por Id (Administrador y Editor)
- Aprobar comentario por Id (Administrador y Editor) (IsApproved = true)
![](https://i.ibb.co/T4HJSLM/image.png)

# DTO para la administración de datos de entrata y salida
![](https://i.ibb.co/vP0xywq/swagger-DTO-backend-blog.png)

# Dependencias
- Entity Framework
- Entity Framwork SQLServer
- Entity Framwork Core
- Entity Framwork Tools
- AutoMapper
- FluentValidation
- MailKit
- Jwt Bearer
- Tokens
- Newtonsoft Json
