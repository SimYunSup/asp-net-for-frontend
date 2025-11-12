# Chapter 18: API 클라이언트 패턴 - 타입 안전성과 선언적 데이터 페칭

## 프론트엔드-백엔드 통합의 성배

프론트엔드 개발자로서 여러분은 이런 경험이 있을 것입니다. 백엔드 API가 변경되었는데, 문서가 업데이트되지 않았습니다. 여러분은 Postman으로 실제 응답을 확인하고, TypeScript 인터페이스를 수동으로 수정합니다. 하지만 한 곳을 빠뜨렸고, 프로덕션에서 `undefined`를 읽다가 에러가 발생합니다. 사용자들이 불평하고, 긴급 롤백을 합니다.

또 다른 시나리오. API 호출이 느립니다. 사용자가 같은 페이지를 여러 번 방문하는데, 매번 서버에 요청합니다. 캐싱을 구현하고 싶지만, 어디서 어떻게 해야 할지 막막합니다. `localStorage`? 메모리? 서버 캐시? 모든 계층을 조합하면 어떻게 될까요?

이 챕터는 이러한 문제를 근본적으로 해결합니다. **타입 안전한 API 클라이언트 자동 생성**으로 서버 타입 변경이 즉시 프론트엔드 컴파일 에러로 이어지게 하고, **선언적 데이터 페칭 라이브러리**로 로딩, 에러, 캐싱을 자동으로 처리하며, **다계층 캐싱 전략**으로 최적의 성능을 얻습니다.

## Part 1: 타입 안전한 API 클라이언트 생성

### 문제: 런타임 타입 불일치

TypeScript는 타입 안전성을 제공하지만, API 경계에서 무너집니다.

```typescript
// 수동으로 작성한 인터페이스
interface User {
  id: number;
  name: string;
  email: string;
  createdAt: string;
}

async function fetchUser(id: number): Promise<User> {
  const response = await fetch(`/api/users/${id}`);
  return await response.json(); // 타입 캐스팅, 실제로는 검증 안 됨!
}

// 사용
const user = await fetchUser(1);
console.log(user.email); // email 필드가 제거되었다면? 런타임 에러!
```

TypeScript는 `response.json()`이 정말로 `User` 타입을 반환하는지 검증하지 않습니다. 컴파일 타임에는 문제가 없지만, 런타임에 API가 다른 구조를 반환하면 에러가 발생합니다.

더 나쁜 것은, API가 변경되어도 TypeScript가 알 수 없다는 점입니다. 백엔드에서 `email` 필드를 제거하고 `emailAddress`로 변경했다면? 프론트엔드 코드는 여전히 컴파일되지만, 프로덕션에서 `undefined`를 만나게 됩니다.

### 해결책: OpenAPI 명세에서 클라이언트 생성

진정한 타입 안전성은 **서버의 스키마에서 클라이언트 코드를 자동 생성**하는 것입니다. 서버의 API 정의가 단일 진실 소스(Single Source of Truth)가 되며, 클라이언트는 이를 따릅니다.

ASP.NET Core API는 OpenAPI(Swagger) 명세를 자동으로 생성합니다. 이 명세는 JSON 형식으로 모든 엔드포인트, 요청/응답 타입, HTTP 메서드, 상태 코드를 정확히 기술합니다.

```json
{
  "openapi": "3.0.1",
  "paths": {
    "/api/users/{id}": {
      "get": {
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "required": true,
            "schema": { "type": "integer" }
          }
        ],
        "responses": {
          "200": {
            "content": {
              "application/json": {
                "schema": { "$ref": "#/components/schemas/User" }
              }
            }
          }
        }
      }
    }
  },
  "components": {
    "schemas": {
      "User": {
        "type": "object",
        "properties": {
          "id": { "type": "integer" },
          "name": { "type": "string" },
          "email": { "type": "string" },
          "createdAt": { "type": "string", "format": "date-time" }
        }
      }
    }
  }
}
```

이 명세에서 TypeScript 클라이언트를 생성하면, 서버 타입과 완벽하게 일치하는 코드를 얻습니다.

### NSwag: OpenAPI 코드 생성기

**NSwag**는 OpenAPI 명세에서 TypeScript, C#, 기타 언어의 클라이언트를 생성하는 도구입니다.

```bash
# NSwagStudio 다운로드 또는 CLI 설치
dotnet tool install -g NSwag.ConsoleCore
```

**nswag.json 설정 파일:**

```json
{
  "runtime": "Net80",
  "defaultVariables": null,
  "documentGenerator": {
    "aspNetCoreToOpenApi": {
      "project": "../MyApi/MyApi.csproj",
      "msBuildProjectExtensionsPath": null,
      "configuration": null,
      "runtime": null,
      "targetFramework": null,
      "noBuild": false,
      "verbose": true,
      "workingDirectory": null,
      "requireParametersWithoutDefault": true,
      "apiGroupNames": null,
      "defaultPropertyNameHandling": "Default",
      "defaultReferenceTypeNullHandling": "Null",
      "defaultDictionaryValueReferenceTypeNullHandling": "NotNull",
      "defaultResponseReferenceTypeNullHandling": "NotNull",
      "defaultEnumHandling": "Integer",
      "flattenInheritanceHierarchy": false,
      "generateKnownTypes": true,
      "generateEnumMappingDescription": false,
      "generateXmlObjects": false,
      "generateAbstractProperties": false,
      "generateAbstractSchemas": true,
      "ignoreObsoleteProperties": false,
      "allowReferencesWithProperties": false,
      "excludedTypeNames": [],
      "serviceHost": null,
      "serviceBasePath": null,
      "serviceSchemes": [],
      "infoTitle": "My API",
      "infoDescription": null,
      "infoVersion": "1.0.0",
      "documentTemplate": null,
      "documentProcessorTypes": [],
      "operationProcessorTypes": [],
      "typeNameGeneratorType": null,
      "schemaNameGeneratorType": null,
      "contractResolverType": null,
      "serializerSettingsType": null,
      "useDocumentProvider": true,
      "documentName": "v1",
      "aspNetCoreEnvironment": null,
      "createWebHostBuilderMethod": null,
      "startupType": null,
      "allowNullableBodyParameters": true,
      "output": null,
      "outputType": "Swagger2"
    }
  },
  "codeGenerators": {
    "openApiToTypeScriptClient": {
      "className": "{controller}Client",
      "moduleName": "",
      "namespace": "",
      "typeScriptVersion": 4.3,
      "template": "Fetch",
      "promiseType": "Promise",
      "httpClass": "HttpClient",
      "withCredentials": false,
      "useSingletonProvider": false,
      "injectionTokenType": "InjectionToken",
      "rxJsVersion": 7.0,
      "dateTimeType": "Date",
      "nullValue": "Undefined",
      "generateClientClasses": true,
      "generateClientInterfaces": false,
      "generateOptionalParameters": false,
      "exportTypes": true,
      "wrapDtoExceptions": true,
      "exceptionClass": "ApiException",
      "clientBaseClass": null,
      "wrapResponses": false,
      "wrapResponseMethods": [],
      "generateResponseClasses": true,
      "responseClass": "SwaggerResponse",
      "protectedMethods": [],
      "configurationClass": null,
      "useTransformOptionsMethod": false,
      "useTransformResultMethod": false,
      "generateDtoTypes": true,
      "operationGenerationMode": "MultipleClientsFromOperationId",
      "markOptionalProperties": true,
      "generateCloneMethod": false,
      "typeStyle": "Interface",
      "classTypes": [],
      "extendedClasses": [],
      "extensionCode": null,
      "generateDefaultValues": true,
      "excludedTypeNames": [],
      "excludedParameterNames": [],
      "handleReferences": false,
      "generateConstructorInterface": true,
      "convertConstructorInterfaceData": false,
      "importRequiredTypes": true,
      "useGetBaseUrlMethod": false,
      "baseUrlTokenName": "API_BASE_URL",
      "queryNullValue": "",
      "inlineNamedDictionaries": false,
      "inlineNamedAny": false,
      "templateDirectory": null,
      "typeNameGeneratorType": null,
      "propertyNameGeneratorType": null,
      "enumNameGeneratorType": null,
      "serviceHost": null,
      "serviceSchemes": null,
      "output": "../frontend/src/api/client.ts"
    }
  }
}
```

**생성 실행:**

```bash
nswag run nswag.json
```

**생성된 클라이언트 사용:**

```typescript
import { UserClient, User } from './api/client';

const apiBaseUrl = 'https://localhost:5001';
const client = new UserClient(apiBaseUrl);

// 타입 안전한 호출
const user: User = await client.getUser(1);
console.log(user.email); // 타입이 보장됨!

// email 필드가 서버에서 제거되면?
// TypeScript 컴파일 에러: Property 'email' does not exist on type 'User'
```

서버의 `User` 모델에서 `email` 필드가 제거되면:

1. OpenAPI 명세가 업데이트됨
2. 클라이언트 재생성 시 TypeScript 타입도 업데이트됨
3. `user.email`을 사용하는 모든 코드가 컴파일 에러 발생
4. 배포 전에 문제 발견!

### MSBuild 통합: 자동 생성

매번 수동으로 클라이언트를 재생성하는 것은 번거롭습니다. MSBuild 타겟으로 자동화할 수 있습니다.

```xml
<!-- frontend.csproj 또는 별도의 .targets 파일 -->
<Target Name="GenerateApiClient" BeforeTargets="BeforeBuild">
  <Exec Command="nswag run nswag.json" WorkingDirectory="$(MSBuildProjectDirectory)" />
</Target>
```

이제 백엔드 프로젝트를 빌드할 때마다 클라이언트가 자동으로 재생성됩니다.

### Kiota: Microsoft의 차세대 생성기

**Kiota**는 Microsoft의 새로운 API 클라이언트 생성기입니다. NSwag보다 더 현대적인 API와 fluent 스타일을 제공합니다.

```bash
dotnet tool install -g Microsoft.OpenApi.Kiota
```

```bash
kiota generate -l TypeScript -c MyApiClient -n MyApp.ApiClient -d https://localhost:5001/swagger/v1/swagger.json -o ./src/api
```

**생성된 Kiota 클라이언트:**

```typescript
import { MyApiClient } from './api/myApiClient';

const client = new MyApiClient('https://localhost:5001');

// Fluent API 스타일
const user = await client.users.byUserId(1).get();
const posts = await client.users.byUserId(1).posts.get();

// 쿼리 파라미터
const users = await client.users.get({
  queryParameters: {
    page: 1,
    pageSize: 10,
    search: 'john'
  }
});

// POST 요청
const newUser = await client.users.post({
  name: 'John Doe',
  email: 'john@example.com'
});
```

Kiota의 fluent API는 REST의 계층 구조를 자연스럽게 표현하며, IDE의 자동 완성이 API 탐색을 돕습니다.

## Part 2: 선언적 데이터 페칭 - React Query 통합

### 전통적인 데이터 페칭의 문제

생성된 클라이언트는 타입 안전하지만, 여전히 저수준입니다. 로딩 상태, 에러 처리, 캐싱, 재시도를 직접 구현해야 합니다.

```typescript
function UserProfile({ userId }: { userId: number }) {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<Error | null>(null);

  useEffect(() => {
    const client = new UserClient(API_BASE_URL);

    setLoading(true);
    client.getUser(userId)
      .then(setUser)
      .catch(setError)
      .finally(() => setLoading(false));
  }, [userId]);

  if (loading) return <div>Loading...</div>;
  if (error) return <div>Error: {error.message}</div>;
  if (!user) return null;

  return <div>{user.name}</div>;
}
```

이 코드는 작동하지만, 보일러플레이트가 많고 캐싱이 없으며 재시도 로직도 없습니다.

### React Query: 선언적 데이터 페칭

**React Query** (TanStack Query)는 서버 상태 관리를 혁신적으로 단순화합니다.

```bash
npm install @tanstack/react-query
```

```typescript
import { QueryClient, QueryClientProvider, useQuery } from '@tanstack/react-query';

// QueryClient 생성
const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 5 * 60 * 1000, // 5분
      cacheTime: 10 * 60 * 1000, // 10분
      retry: 3,
      refetchOnWindowFocus: false,
    },
  },
});

// App에서 Provider 설정
function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <UserProfile userId={1} />
    </QueryClientProvider>
  );
}

// 생성된 클라이언트 + React Query
const userClient = new UserClient(API_BASE_URL);

function UserProfile({ userId }: { userId: number }) {
  const { data: user, isLoading, error } = useQuery({
    queryKey: ['user', userId],
    queryFn: () => userClient.getUser(userId),
  });

  if (isLoading) return <div>Loading...</div>;
  if (error) return <div>Error: {error.message}</div>;
  if (!user) return null;

  return <div>{user.name}</div>;
}
```

이제 로딩, 에러, 캐싱이 모두 자동입니다. 같은 사용자를 다시 요청하면, React Query는 캐시된 데이터를 즉시 반환합니다.

### Mutation: 데이터 수정

```typescript
import { useMutation, useQueryClient } from '@tanstack/react-query';

function UpdateUserForm({ userId }: { userId: number }) {
  const queryClient = useQueryClient();

  const { mutate: updateUser, isPending } = useMutation({
    mutationFn: (data: UpdateUserDto) => userClient.updateUser(userId, data),
    onSuccess: (updatedUser) => {
      // 캐시 무효화
      queryClient.invalidateQueries({ queryKey: ['user', userId] });

      // 또는 직접 업데이트
      queryClient.setQueryData(['user', userId], updatedUser);

      toast.success('User updated!');
    },
    onError: (error) => {
      toast.error(`Update failed: ${error.message}`);
    },
  });

  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const formData = new FormData(e.currentTarget);

    updateUser({
      name: formData.get('name') as string,
      email: formData.get('email') as string,
    });
  };

  return (
    <form onSubmit={handleSubmit}>
      <input name="name" placeholder="Name" />
      <input name="email" type="email" placeholder="Email" />
      <button type="submit" disabled={isPending}>
        {isPending ? 'Updating...' : 'Update'}
      </button>
    </form>
  );
}
```

`invalidateQueries`는 캐시를 무효화하여 다음 조회 시 서버에서 새 데이터를 가져오게 합니다. `setQueryData`는 캐시를 직접 업데이트하여 서버 요청 없이 UI를 즉시 갱신합니다.

### Optimistic UI 업데이트: 즉각적인 사용자 경험

사용자가 버튼을 클릭하면, 서버 응답을 기다리지 않고 즉시 UI를 업데이트합니다. 서버 요청이 실패하면 롤백합니다.

```typescript
function TodoList() {
  const queryClient = useQueryClient();

  const { data: todos } = useQuery({
    queryKey: ['todos'],
    queryFn: () => todoClient.getTodos(),
  });

  const { mutate: toggleTodo } = useMutation({
    mutationFn: (todoId: number) => todoClient.toggleTodo(todoId),

    // Optimistic 업데이트
    onMutate: async (todoId) => {
      // 진행 중인 리페치 취소
      await queryClient.cancelQueries({ queryKey: ['todos'] });

      // 이전 상태 스냅샷
      const previousTodos = queryClient.getQueryData<Todo[]>(['todos']);

      // Optimistic 업데이트
      queryClient.setQueryData<Todo[]>(['todos'], (old) =>
        old?.map((todo) =>
          todo.id === todoId
            ? { ...todo, completed: !todo.completed }
            : todo
        )
      );

      // 롤백을 위해 이전 상태 반환
      return { previousTodos };
    },

    // 에러 시 롤백
    onError: (err, todoId, context) => {
      if (context?.previousTodos) {
        queryClient.setQueryData(['todos'], context.previousTodos);
      }
      toast.error('Failed to toggle todo');
    },

    // 성공 시 서버 데이터로 재동기화
    onSettled: () => {
      queryClient.invalidateQueries({ queryKey: ['todos'] });
    },
  });

  return (
    <ul>
      {todos?.map((todo) => (
        <li key={todo.id}>
          <input
            type="checkbox"
            checked={todo.completed}
            onChange={() => toggleTodo(todo.id)}
          />
          {todo.title}
        </li>
      ))}
    </ul>
  );
}
```

이 패턴은 사용자에게 즉각적인 피드백을 제공합니다. 체크박스를 클릭하면 즉시 상태가 변경되며, 서버 요청이 백그라운드에서 진행됩니다. 실패하면 자동으로 롤백되고, 에러 메시지가 표시됩니다.

### 페이지네이션과 무한 스크롤

```typescript
// 페이지네이션
function UserList() {
  const [page, setPage] = useState(1);

  const { data, isLoading } = useQuery({
    queryKey: ['users', page],
    queryFn: () => userClient.getUsers({ page, pageSize: 10 }),
    placeholderData: (previousData) => previousData, // 이전 데이터 유지
  });

  return (
    <div>
      {isLoading && <div>Loading...</div>}
      {data?.items.map((user) => (
        <div key={user.id}>{user.name}</div>
      ))}
      <button onClick={() => setPage((p) => p - 1)} disabled={page === 1}>
        Previous
      </button>
      <button onClick={() => setPage((p) => p + 1)} disabled={!data?.hasMore}>
        Next
      </button>
    </div>
  );
}

// 무한 스크롤
import { useInfiniteQuery } from '@tanstack/react-query';

function InfiniteUserList() {
  const {
    data,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
  } = useInfiniteQuery({
    queryKey: ['users', 'infinite'],
    queryFn: ({ pageParam = 1 }) =>
      userClient.getUsers({ page: pageParam, pageSize: 20 }),
    getNextPageParam: (lastPage, allPages) =>
      lastPage.hasMore ? allPages.length + 1 : undefined,
    initialPageParam: 1,
  });

  return (
    <div>
      {data?.pages.map((page, i) => (
        <React.Fragment key={i}>
          {page.items.map((user) => (
            <div key={user.id}>{user.name}</div>
          ))}
        </React.Fragment>
      ))}

      {hasNextPage && (
        <button onClick={() => fetchNextPage()} disabled={isFetchingNextPage}>
          {isFetchingNextPage ? 'Loading more...' : 'Load More'}
        </button>
      )}
    </div>
  );
}
```

## Part 3: 다계층 캐싱 전략

캐싱은 여러 계층에서 적용할 수 있습니다. 각 계층은 서로 다른 트레이드오프를 가지며, 함께 사용될 때 최적의 성능을 냅니다.

### Layer 1: 브라우저 캐싱 (HTTP 캐시 헤더)

가장 빠른 캐시는 서버에 요청조차 보내지 않는 것입니다.

```csharp
[HttpGet("{id}")]
[ResponseCache(Duration = 3600, VaryByQueryKeys = new[] { "id" })]
public IActionResult GetUser(int id)
{
    var user = _context.Users.Find(id);
    if (user == null) return NotFound();

    // ETag 생성 (엔티티 버전 기반)
    var etag = $"\"{user.UpdatedAt.Ticks}\"";
    Response.Headers.ETag = etag;

    // 클라이언트의 If-None-Match 헤더 확인
    if (Request.Headers.IfNoneMatch == etag)
    {
        return StatusCode(304); // Not Modified
    }

    return Ok(user);
}
```

브라우저는 `Cache-Control: max-age=3600` 헤더를 보고, 1시간 동안 캐시된 응답을 사용합니다. 1시간 후 서버에 요청할 때는 `If-None-Match: "<etag>"` 헤더를 보내며, ETag가 일치하면 서버는 `304 Not Modified`만 응답하고 본문은 보내지 않습니다.

### Layer 2: 클라이언트 사이드 캐싱 (React Query)

React Query는 메모리에 데이터를 캐시합니다.

```typescript
const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 5 * 60 * 1000, // 5분 동안 fresh
      cacheTime: 10 * 60 * 1000, // 10분 동안 메모리에 유지
      refetchOnWindowFocus: true, // 창 포커스 시 재검증
      refetchOnReconnect: true, // 재연결 시 재검증
    },
  },
});
```

- **staleTime**: 데이터가 "신선한" 상태로 유지되는 시간. 이 시간 동안은 서버에 요청하지 않고 캐시 반환.
- **cacheTime**: 데이터가 메모리에 유지되는 시간. 사용하지 않는 데이터도 이 시간 동안은 캐시에 남음.

### Layer 3: 서버 사이드 메모리 캐싱 (IMemoryCache)

서버 메모리에 데이터를 캐시하여 데이터베이스 쿼리를 건너뜁니다.

```csharp
public class UserService
{
    private readonly IMemoryCache _cache;
    private readonly AppDbContext _context;

    public async Task<User?> GetUserAsync(int id)
    {
        var cacheKey = $"user_{id}";

        if (_cache.TryGetValue(cacheKey, out User? cachedUser))
        {
            return cachedUser;
        }

        var user = await _context.Users.FindAsync(id);

        if (user != null)
        {
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(10))
                .SetSlidingExpiration(TimeSpan.FromMinutes(2));

            _cache.Set(cacheKey, user, cacheOptions);
        }

        return user;
    }

    public void InvalidateUser(int id)
    {
        _cache.Remove($"user_{id}");
    }
}
```

### Layer 4: 분산 캐싱 (IDistributedCache + Redis)

여러 서버가 캐시를 공유합니다.

```csharp
public class UserService
{
    private readonly IDistributedCache _cache;
    private readonly AppDbContext _context;

    public async Task<User?> GetUserAsync(int id)
    {
        var cacheKey = $"user_{id}";
        var cached = await _cache.GetStringAsync(cacheKey);

        if (cached != null)
        {
            return JsonSerializer.Deserialize<User>(cached);
        }

        var user = await _context.Users.FindAsync(id);

        if (user != null)
        {
            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(user),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
        }

        return user;
    }
}

// Program.cs
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "MyApp_";
});
```

### Layer 5: HybridCache (.NET 9) - 최선의 조합

.NET 9의 `HybridCache`는 L1(메모리) + L2(분산) 캐시를 자동으로 조합하며, stampede 문제도 방지합니다.

```csharp
public class UserService
{
    private readonly HybridCache _cache;
    private readonly AppDbContext _context;

    public async Task<User?> GetUserAsync(int id, CancellationToken token = default)
    {
        return await _cache.GetOrCreateAsync(
            $"user_{id}",
            async cancel =>
            {
                return await _context.Users.FindAsync(new object[] { id }, cancel);
            },
            new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(10), // L2 만료
                LocalCacheExpiration = TimeSpan.FromMinutes(2) // L1 만료
            },
            token);
    }
}

// Program.cs
builder.Services.AddHybridCache(options =>
{
    options.MaximumPayloadBytes = 1024 * 1024; // 1MB
    options.MaximumKeyLength = 1024;
    options.DefaultEntryOptions = new HybridCacheEntryOptions
    {
        Expiration = TimeSpan.FromMinutes(5),
        LocalCacheExpiration = TimeSpan.FromMinutes(1)
    };
});
```

HybridCache는 먼저 L1(메모리)에서 찾고, 없으면 L2(Redis)에서 찾으며, 모두 없으면 팩토리 함수를 실행합니다. 여러 요청이 동시에 같은 키를 요청해도, 팩토리는 한 번만 실행됩니다(stampede 방지).

### 캐시 무효화 전략

캐시의 가장 어려운 부분은 무효화입니다. 데이터가 변경되었을 때 캐시를 어떻게 갱신할까요?

**1. Time-based Expiration**: 가장 단순. 일정 시간 후 자동 만료.

```csharp
_cache.Set(key, value, TimeSpan.FromMinutes(10));
```

**2. Explicit Invalidation**: 데이터 변경 시 명시적으로 캐시 제거.

```csharp
public async Task UpdateUserAsync(int id, UpdateUserDto dto)
{
    var user = await _context.Users.FindAsync(id);
    // ... 업데이트 로직

    await _context.SaveChangesAsync();

    // 캐시 무효화
    _cache.Remove($"user_{id}");
}
```

**3. Tag-based Invalidation**: 관련된 캐시를 그룹으로 무효화.

```csharp
// HybridCache는 태그를 지원합니다 (.NET 9)
await _cache.GetOrCreateAsync(
    $"user_{id}",
    factory,
    new HybridCacheEntryOptions
    {
        Expiration = TimeSpan.FromMinutes(10),
        Tags = new[] { "users", $"user_{id}" }
    });

// 특정 태그의 모든 캐시 무효화
await _cache.RemoveByTagAsync("users");
```

**4. Stale-While-Revalidate**: 만료된 캐시를 반환하면서 백그라운드에서 갱신.

React Query는 이를 내장합니다:

```typescript
const { data } = useQuery({
  queryKey: ['user', userId],
  queryFn: () => userClient.getUser(userId),
  staleTime: 5 * 60 * 1000, // 5분 후 stale
  cacheTime: 10 * 60 * 1000, // 10분 동안 캐시 유지
});

// 5분 후: stale 데이터를 즉시 반환하면서, 백그라운드에서 새 데이터 페칭
```

## 핵심 교훈

1. **타입 안전성**: OpenAPI에서 클라이언트 자동 생성
2. **선언적 페칭**: React Query로 로딩/에러/캐싱 자동화
3. **Optimistic UI**: 즉각적인 피드백, 실패 시 롤백
4. **다계층 캐싱**: 브라우저 → 클라이언트 → 서버 메모리 → 분산 캐시
5. **무효화 전략**: 데이터 변경 시 캐시 갱신

프론트엔드와 백엔드의 완벽한 통합은 더 이상 꿈이 아닙니다. 자동 생성된 타입 안전 클라이언트, 선언적 데이터 페칭, 지능적인 캐싱—이 모든 것이 함께 작동하여, 개발자 경험과 사용자 경험을 모두 극대화합니다.

다음 챕터에서는 서버 사이드 상태 관리와 고급 아키텍처 패턴을 배웁니다. Clean Architecture, CQRS, Domain-Driven Design... 대규모 애플리케이션을 유지보수 가능하게 만드는 패턴들이 기다립니다.
