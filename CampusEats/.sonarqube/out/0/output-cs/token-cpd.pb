´
|C:\Users\Veronica\Desktop\dot_Net\CampusEats\CampusEatsProject_dotNET\CampusEats\CampusEats.Frontend\Services\UserService.cs
	namespace 	
CampusEatsFrontend
 
. 
Services %
;% &
public 
class 
UserService 
{		 
private

 
readonly

 

HttpClient

 
_http

  %
;

% &
private 
readonly !
JsonSerializerOptions *
_jsonOptions+ 7
;7 8
public 

UserService 
( 

HttpClient !
http" &
)& '
{ 
_http 
= 
http 
; 
_jsonOptions 
= 
new !
JsonSerializerOptions 0
{ 	'
PropertyNameCaseInsensitive '
=( )
true* .
,. /

Converters 
= 
{ 
new #
JsonStringEnumConverter 6
(6 7
)7 8
}9 :
} 	
;	 

} 
public 

async 
Task 
< 
List 
< 
ClientProfileDto +
>+ ,
?, -
>- .
GetAllUsersAsync/ ?
(? @
)@ A
{ 
try 
{ 	
return 
await 
_http 
. 
GetFromJsonAsync /
</ 0
List0 4
<4 5
ClientProfileDto5 E
>E F
>F G
(G H
$strH S
,S T
_jsonOptionsU a
)a b
;b c
} 	
catch 
{ 	
return 
new 
List 
< 
ClientProfileDto ,
>, -
(- .
). /
;/ 0
}   	
}!! 
public## 

async## 
Task## 
<## 
ClientProfileDto## &
?##& '
>##' (
GetUserByIdAsync##) 9
(##9 :
Guid##: >
userId##? E
)##E F
{$$ 
try%% 
{&& 	
return'' 
await'' 
_http'' 
.'' 
GetFromJsonAsync'' /
<''/ 0
ClientProfileDto''0 @
>''@ A
(''A B
$"''B D
$str''D N
{''N O
userId''O U
}''U V
"''V W
,''W X
_jsonOptions''Y e
)''e f
;''f g
}(( 	
catch)) 
{** 	
return++ 
null++ 
;++ 
},, 	
}-- 
}.. ≤ 
C:\Users\Veronica\Desktop\dot_Net\CampusEats\CampusEatsProject_dotNET\CampusEats\CampusEats.Frontend\Services\PaymentService.cs
	namespace 	
CampusEatsFrontend
 
. 
Services %
;% &
public 
class 
PaymentService 
{		 
private

 
readonly

 

HttpClient

 
_http

  %
;

% &
private 
readonly !
JsonSerializerOptions *
_jsonOptions+ 7
;7 8
public 

PaymentService 
( 

HttpClient $
http% )
)) *
{ 
_http 
= 
http 
; 
_jsonOptions 
= 
new !
JsonSerializerOptions 0
{ 	'
PropertyNameCaseInsensitive '
=( )
true* .
,. /

Converters 
= 
{ 
new #
JsonStringEnumConverter 6
(6 7
)7 8
}9 :
} 	
;	 

} 
public 

async 
Task 
< 
HttpResponseMessage )
>) *
CreatePaymentAsync+ =
(= > 
CreatePaymentRequest> R
requestS Z
)Z [
{ 
return 
await 
_http 
. 
PostAsJsonAsync *
(* +
$str+ 9
,9 :
request; B
,B C
_jsonOptionsD P
)P Q
;Q R
} 
public 

async 
Task 
< 

PaymentDto  
?  !
>! "
GetPaymentByIdAsync# 6
(6 7
Guid7 ;
id< >
)> ?
{ 
return 
await 
_http 
. 
GetFromJsonAsync +
<+ ,

PaymentDto, 6
>6 7
(7 8
$"8 :
$str: G
{G H
idH J
}J K
"K L
,L M
_jsonOptionsN Z
)Z [
;[ \
} 
public!! 

async!! 
Task!! 
<!! 
List!! 
<!! 

PaymentDto!! %
>!!% &
?!!& '
>!!' ("
GetPaymentHistoryAsync!!) ?
(!!? @
Guid!!@ D
userId!!E K
)!!K L
{"" 
return## 
await## 
_http## 
.## 
GetFromJsonAsync## +
<##+ ,
List##, 0
<##0 1

PaymentDto##1 ;
>##; <
>##< =
(##= >
$"##> @
$str##@ J
{##J K
userId##K Q
}##Q R
$str##R [
"##[ \
,##\ ]
_jsonOptions##^ j
)##j k
;##k l
}$$ 
public&& 

async&& 
Task&& 
<&& 
string&& 
?&& 
>&& ,
 CreateStripeCheckoutSessionAsync&& ?
(&&? @.
"CreateStripeCheckoutSessionRequest&&@ b
request&&c j
)&&j k
{'' 
var(( 
response(( 
=(( 
await(( 
_http(( "
.((" #
PostAsJsonAsync((# 2
(((2 3
$str((3 Y
,((Y Z
request(([ b
,((b c
_jsonOptions((d p
)((p q
;((q r
if)) 

()) 
!)) 
response)) 
.)) 
IsSuccessStatusCode)) )
)))) *
return** 
null** 
;** 
var,, 
payload,, 
=,, 
await,, 
response,, $
.,,$ %
Content,,% ,
.,,, -
ReadFromJsonAsync,,- >
<,,> ?)
StripeCheckoutSessionResponse,,? \
>,,\ ]
(,,] ^
_jsonOptions,,^ j
),,j k
;,,k l
return-- 
payload-- 
?-- 
.-- 
CheckoutUrl-- #
;--# $
}.. 
}// ‘'
}C:\Users\Veronica\Desktop\dot_Net\CampusEats\CampusEatsProject_dotNET\CampusEats\CampusEats.Frontend\Services\OrderService.cs
	namespace 	
CampusEatsFrontend
 
. 
Services %
;% &
public 
class 
OrderService 
{		 
private

 
readonly

 

HttpClient

 
_http

  %
;

% &
private 
readonly !
JsonSerializerOptions *
_jsonOptions+ 7
;7 8
public 

OrderService 
( 

HttpClient "
http# '
)' (
{ 
_http 
= 
http 
; 
_jsonOptions 
= 
new !
JsonSerializerOptions 0
{ 	'
PropertyNameCaseInsensitive '
=( )
true* .
,. /

Converters 
= 
{ 
new #
JsonStringEnumConverter 6
(6 7
)7 8
}9 :
} 	
;	 

} 
public 

async 
Task 
< 
OrderDto 
? 
>  
GetOrderByIdAsync! 2
(2 3
Guid3 7
id8 :
): ;
{ 
return 
await 
_http 
. 
GetFromJsonAsync +
<+ ,
OrderDto, 4
>4 5
(5 6
$"6 8
$str8 C
{C D
idD F
}F G
"G H
,H I
_jsonOptionsJ V
)V W
;W X
} 
public 

async 
Task 
< 
List 
< 
OrderDto #
># $
?$ %
>% & 
GetOrderHistoryAsync' ;
(; <
Guid< @
clientIdA I
)I J
{ 
return 
await 
_http 
. 
GetFromJsonAsync +
<+ ,
List, 0
<0 1
OrderDto1 9
>9 :
>: ;
(; <
$"< >
$str> J
{J K
clientIdK S
}S T
$strT [
"[ \
,\ ]
_jsonOptions^ j
)j k
;k l
} 
public!! 

async!! 
Task!! 
<!! 
HttpResponseMessage!! )
>!!) *
PlaceOrderAsync!!+ :
(!!: ;
PlaceOrderRequest!!; L
request!!M T
)!!T U
{"" 
return## 
await## 
_http## 
.## 
PostAsJsonAsync## *
(##* +
$str##+ 7
,##7 8
request##9 @
,##@ A
_jsonOptions##B N
)##N O
;##O P
}$$ 
public&& 

async&& 
Task&& 
<&& 
HttpResponseMessage&& )
>&&) *
CancelOrderAsync&&+ ;
(&&; <
Guid&&< @
orderId&&A H
)&&H I
{'' 
return(( 
await(( 
_http(( 
.(( 
	PostAsync(( $
((($ %
$"((% '
$str((' 2
{((2 3
orderId((3 :
}((: ;
$str((; B
"((B C
,((C D
null((E I
)((I J
;((J K
})) 
public,, 

async,, 
Task,, 
<,, 
List,, 
<,, 
OrderDto,, #
>,,# $
?,,$ %
>,,% &
GetAllOrdersAsync,,' 8
(,,8 9
),,9 :
{-- 
try.. 
{// 	
return11 
await11 
_http11 
.11 
GetFromJsonAsync11 /
<11/ 0
List110 4
<114 5
OrderDto115 =
>11= >
>11> ?
(11? @
$str11@ L
,11L M
_jsonOptions11N Z
)11Z [
;11[ \
}22 	
catch33 
{44 	
return55 
new55 
List55 
<55 
OrderDto55 $
>55$ %
(55% &
)55& '
;55' (
}66 	
}77 
public:: 

async:: 
Task:: 
<:: 
OrderDetailsDto:: %
?::% &
>::& ' 
GetOrderDetailsAsync::( <
(::< =
Guid::= A
orderId::B I
)::I J
{;; 
try<< 
{== 	
return>> 
await>> 
_http>> 
.>> 
GetFromJsonAsync>> /
<>>/ 0
OrderDetailsDto>>0 ?
>>>? @
(>>@ A
$">>A C
$str>>C N
{>>N O
orderId>>O V
}>>V W
$str>>W _
">>_ `
,>>` a
_jsonOptions>>b n
)>>n o
;>>o p
}?? 	
catch@@ 
{AA 	
returnBB 
nullBB 
;BB 
}CC 	
}DD 
}EE ∆ 
|C:\Users\Veronica\Desktop\dot_Net\CampusEats\CampusEatsProject_dotNET\CampusEats\CampusEats.Frontend\Services\MenuService.cs
	namespace 	
CampusEatsFrontend
 
. 
Services %
;% &
public 
class 
MenuService 
( 

HttpClient #
http$ (
)( )
{		 
private

 
readonly

 !
JsonSerializerOptions

 *
_jsonOptions

+ 7
=

8 9
new

: =
(

= >
)

> ?
{ '
PropertyNameCaseInsensitive #
=$ %
true& *
,* +

Converters 
= 
{ 
new #
JsonStringEnumConverter 2
(2 3
JsonNamingPolicy3 C
.C D
	CamelCaseD M
)M N
}O P
} 
; 
public 

async 
Task 
< 
List 
< 
MenuDto "
>" #
?# $
>$ %
GetAllMenusAsync& 6
(6 7
)7 8
{ 
return 
await 
http 
. 
GetFromJsonAsync *
<* +
List+ /
</ 0
MenuDto0 7
>7 8
>8 9
(9 :
$str: D
,D E
_jsonOptionsF R
)R S
;S T
} 
public 

async 
Task 
< 
MenuDto 
? 
> 
GetMenuByIdAsync  0
(0 1
Guid1 5
id6 8
)8 9
{ 
return 
await 
http 
. 
GetFromJsonAsync *
<* +
MenuDto+ 2
>2 3
(3 4
$"4 6
$str6 ?
{? @
id@ B
}B C
"C D
,D E
_jsonOptionsF R
)R S
;S T
} 
public 

async 
Task 
< 
MenuDto 
> 
CreateMenuAsync .
(. /
CreateMenuRequest/ @
menuA E
)E F
{ 
var 
response 
= 
await 
http !
.! "
PostAsJsonAsync" 1
(1 2
$str2 <
,< =
menu> B
,B C
_jsonOptionsD P
)P Q
;Q R
response 
. #
EnsureSuccessStatusCode (
(( )
)) *
;* +
return 
await 
response 
. 
Content %
.% &
ReadFromJsonAsync& 7
<7 8
MenuDto8 ?
>? @
(@ A
_jsonOptionsA M
)M N
??O Q
throwR W
newX [
	Exception\ e
(e f
$strf }
)} ~
;~ 
} 
public!! 

async!! 
Task!! 
<!! 
HttpResponseMessage!! )
>!!) *
UpdateMenuAsync!!+ :
(!!: ;
UpdateMenuRequest!!; L
req!!M P
)!!P Q
{"" 
return## 
await## 
http## 
.## 
PutAsJsonAsync## (
(##( )
$"##) +
$str##+ 4
{##4 5
req##5 8
.##8 9
Id##9 ;
}##; <
"##< =
,##= >
req##? B
,##B C
_jsonOptions##D P
)##P Q
;##Q R
}$$ 
public&& 

async&& 
Task&& 
<&& 
HttpResponseMessage&& )
>&&) *
DeleteMenuAsync&&+ :
(&&: ;
Guid&&; ?
id&&@ B
)&&B C
{'' 
return(( 
await(( 
http(( 
.(( 
DeleteAsync(( %
(((% &
$"((& (
$str((( 1
{((1 2
id((2 4
}((4 5
"((5 6
)((6 7
;((7 8
})) 
}** ‚
ÄC:\Users\Veronica\Desktop\dot_Net\CampusEats\CampusEatsProject_dotNET\CampusEats\CampusEats.Frontend\Services\MenuItemService.cs
	namespace 	
CampusEatsFrontend
 
. 
Services %
;% &
public 
class 
MenuItemService 
{		 
private

 
readonly

 

HttpClient

 
_http

  %
;

% &
private 
readonly !
JsonSerializerOptions *
_jsonOptions+ 7
;7 8
public 

MenuItemService 
( 

HttpClient %
http& *
)* +
{ 
_http 
= 
http 
; 
_jsonOptions 
= 
new !
JsonSerializerOptions 0
{ 	'
PropertyNameCaseInsensitive '
=( )
true* .
,. /

Converters 
= 
{ 
new #
JsonStringEnumConverter 6
(6 7
)7 8
}9 :
} 	
;	 

} 
public 

async 
Task 
< 
List 
< 
MenuItemDto &
>& '
?' (
>( ) 
GetAllMenuItemsAsync* >
(> ?
)? @
{ 
return 
await 
_http 
. 
GetFromJsonAsync +
<+ ,
List, 0
<0 1
MenuItemDto1 <
>< =
>= >
(> ?
$str? O
,O P
_jsonOptionsQ ]
)] ^
;^ _
} 
public 

async 
Task 
< 
MenuItemDto !
?! "
>" # 
GetMenuItemByIdAsync$ 8
(8 9
Guid9 =
id> @
)@ A
{ 
return 
await 
_http 
. 
GetFromJsonAsync +
<+ ,
MenuItemDto, 7
>7 8
(8 9
$"9 ;
$str; J
{J K
idK M
}M N
"N O
,O P
_jsonOptionsQ ]
)] ^
;^ _
} 
public!! 

async!! 
Task!! 
<!! 
HttpResponseMessage!! )
>!!) *
CreateMenuItemAsync!!+ >
(!!> ?!
CreateMenuItemRequest!!? T
request!!U \
)!!\ ]
{"" 
return## 
await## 
_http## 
.## 
PostAsJsonAsync## *
(##* +
$str##+ ;
,##; <
request##= D
,##D E
_jsonOptions##F R
)##R S
;##S T
}$$ 
public&& 

async&& 
Task&& 
<&& 
HttpResponseMessage&& )
>&&) *
UpdateMenuItemAsync&&+ >
(&&> ?!
UpdateMenuItemRequest&&? T
request&&U \
)&&\ ]
{'' 
return(( 
await(( 
_http(( 
.(( 
PutAsJsonAsync(( )
((() *
$"((* ,
$str((, ;
{((; <
request((< C
.((C D
Id((D F
}((F G
"((G H
,((H I
request((J Q
,((Q R
_jsonOptions((S _
)((_ `
;((` a
})) 
public++ 

async++ 
Task++ 
<++ 
HttpResponseMessage++ )
>++) *
DeleteMenuItemAsync+++ >
(++> ?
Guid++? C
id++D F
)++F G
{,, 
return-- 
await-- 
_http-- 
.-- 
DeleteAsync-- &
(--& '
$"--' )
$str--) 8
{--8 9
id--9 ;
}--; <
"--< =
)--= >
;--> ?
}.. 
}// “
C:\Users\Veronica\Desktop\dot_Net\CampusEats\CampusEatsProject_dotNET\CampusEats\CampusEats.Frontend\Services\LoyaltyService.cs
	namespace 	
CampusEatsFrontend
 
. 
Services %
;% &
public 
class 
LoyaltyService 
{		 
private

 
readonly

 

HttpClient

 
_http

  %
;

% &
private 
readonly !
JsonSerializerOptions *
_jsonOptions+ 7
;7 8
public 

LoyaltyService 
( 

HttpClient $
http% )
)) *
{ 
_http 
= 
http 
; 
_jsonOptions 
= 
new !
JsonSerializerOptions 0
{ 	'
PropertyNameCaseInsensitive '
=( )
true* .
,. /

Converters 
= 
{ 
new #
JsonStringEnumConverter 6
(6 7
)7 8
}9 :
} 	
;	 

} 
public 

async 
Task 
< 
LoyaltyBalanceDto '
?' (
>( )"
GetLoyaltyBalanceAsync* @
(@ A
GuidA E
userIdF L
)L M
{ 
return 
await 
_http 
. 
GetFromJsonAsync +
<+ ,
LoyaltyBalanceDto, =
>= >
(> ?
$"? A
$strA M
{M N
userIdN T
}T U
$strU ]
"] ^
,^ _
_jsonOptions` l
)l m
;m n
} 
public 

async 
Task 
< 
List 
< !
LoyaltyTransactionDto 0
>0 1
?1 2
>2 3'
GetLoyaltyTransactionsAsync4 O
(O P
GuidP T
userIdU [
)[ \
{ 
return 
await 
_http 
. 
GetFromJsonAsync +
<+ ,
List, 0
<0 1!
LoyaltyTransactionDto1 F
>F G
>G H
(H I
$"I K
$strK W
{W X
userIdX ^
}^ _
$str_ l
"l m
,m n
_jsonOptionso {
){ |
;| }
} 
public!! 

async!! 
Task!! 
<!! 
HttpResponseMessage!! )
>!!) *
RedeemPointsAsync!!+ <
(!!< =
RedeemPointsRequest!!= P
request!!Q X
)!!X Y
{"" 
return## 
await## 
_http## 
.## 
PostAsJsonAsync## *
(##* +
$str##+ ?
,##? @
request##A H
,##H I
_jsonOptions##J V
)##V W
;##W X
}$$ 
}%% ¯"
oC:\Users\Veronica\Desktop\dot_Net\CampusEats\CampusEatsProject_dotNET\CampusEats\CampusEats.Frontend\Program.cs
var 
builder 
= "
WebAssemblyHostBuilder $
.$ %
CreateDefault% 2
(2 3
args3 7
)7 8
;8 9
builder		 
.		 
RootComponents		 
.		 
Add		 
<		 
App		 
>		 
(		  
$str		  &
)		& '
;		' (
builder

 
.

 
RootComponents

 
.

 
Add

 
<

 

HeadOutlet

 %
>

% &
(

& '
$str

' 4
)

4 5
;

5 6
builder 
. 
Services 
. 
	AddScoped 
< '
AuthorizationMessageHandler 6
>6 7
(7 8
)8 9
;9 :
builder 
. 
Services 
. 
	AddScoped 
( 
sp 
=>  
{ 
var 
authHandler 
= 
sp 
. 
GetRequiredService +
<+ ,'
AuthorizationMessageHandler, G
>G H
(H I
)I J
;J K
authHandler 
. 
InnerHandler 
= 
new "
HttpClientHandler# 4
(4 5
)5 6
;6 7
var 

httpClient 
= 
new 

HttpClient #
(# $
authHandler$ /
)/ 0
{ 
BaseAddress 
= 
new 
Uri 
( 
$str 6
)6 7
} 
; 
return 


httpClient 
; 
} 
) 
; 
builder 
. 
Services 
. 
	Configure 
< !
JsonSerializerOptions 0
>0 1
(1 2
options2 9
=>: <
{ 
options 
. '
PropertyNameCaseInsensitive '
=( )
true* .
;. /
options   
.   

Converters   
.   
Add   
(   
new   #
JsonStringEnumConverter   6
(  6 7
)  7 8
)  8 9
;  9 :
}!! 
)!! 
;!! 
builder## 
.## 
Services## 
.## 
	AddScoped## 
<## 
MenuService## &
>##& '
(##' (
)##( )
;##) *
builder$$ 
.$$ 
Services$$ 
.$$ 
	AddScoped$$ 
<$$ 
MenuItemService$$ *
>$$* +
($$+ ,
)$$, -
;$$- .
builder%% 
.%% 
Services%% 
.%% 
	AddScoped%% 
<%% 
OrderService%% '
>%%' (
(%%( )
)%%) *
;%%* +
builder&& 
.&& 
Services&& 
.&& 
	AddScoped&& 
<&& 
KitchenService&& )
>&&) *
(&&* +
)&&+ ,
;&&, -
builder'' 
.'' 
Services'' 
.'' 
	AddScoped'' 
<'' 
AuthService'' &
>''& '
(''' (
)''( )
;'') *
builder(( 
.(( 
Services(( 
.(( 
	AddScoped(( 
<(( 
LoyaltyService(( )
>(() *
(((* +
)((+ ,
;((, -
builder)) 
.)) 
Services)) 
.)) 
	AddScoped)) 
<)) 
PaymentService)) )
>))) *
())* +
)))+ ,
;)), -
builder** 
.** 
Services** 
.** 
	AddScoped** 
<** 
UserService** &
>**& '
(**' (
)**( )
;**) *
builder++ 
.++ 
Services++ 
.++ 
	AddScoped++ 
<++ 
CartService++ &
>++& '
(++' (
)++( )
;++) *
await-- 
builder-- 
.-- 
Build-- 
(-- 
)-- 
.-- 
RunAsync-- 
(-- 
)--  
;--  !µ
C:\Users\Veronica\Desktop\dot_Net\CampusEats\CampusEatsProject_dotNET\CampusEats\CampusEats.Frontend\Services\KitchenService.cs
	namespace 	
CampusEatsFrontend
 
. 
Services %
;% &
public 
class 
KitchenService 
{		 
private

 
readonly

 

HttpClient

 
_http

  %
;

% &
private 
readonly !
JsonSerializerOptions *
_jsonOptions+ 7
;7 8
public 

KitchenService 
( 

HttpClient $
http% )
)) *
{ 
_http 
= 
http 
; 
_jsonOptions 
= 
new !
JsonSerializerOptions 0
{ 	'
PropertyNameCaseInsensitive '
=( )
true* .
,. /

Converters 
= 
{ 
new #
JsonStringEnumConverter 6
(6 7
)7 8
}9 :
} 	
;	 

} 
public 

async 
Task 
< 
List 
< 
OrderDto #
># $
?$ %
>% &!
GetKitchenOrdersAsync' <
(< =
string= C
?C D
statusE K
=L M
nullN R
)R S
{ 
var 
url 
= 
string 
. 
IsNullOrEmpty &
(& '
status' -
)- .
? 
$str "
: 
$" 
$str *
{* +
status+ 1
}1 2
"2 3
;3 4
return 
await 
_http 
. 
GetFromJsonAsync +
<+ ,
List, 0
<0 1
OrderDto1 9
>9 :
>: ;
(; <
url< ?
,? @
_jsonOptionsA M
)M N
;N O
} 
public   

async   
Task   
<   
HttpResponseMessage   )
>  ) *"
UpdateOrderStatusAsync  + A
(  A B
Guid  B F
orderId  G N
,  N O
OrderStatus  P [
	newStatus  \ e
)  e f
{!! 
return"" 
await"" 
_http"" 
."" 
PatchAsJsonAsync"" +
(""+ ,
$""", .
$str"". A
{""A B
orderId""B I
}""I J
$str""J \
{""\ ]
	newStatus""] f
}""f g
"""g h
,""h i
new""j m
{""n o
}""p q
,""q r
_jsonOptions""s 
)	"" Ä
;
""Ä Å
}## 
}$$ †
åC:\Users\Veronica\Desktop\dot_Net\CampusEats\CampusEatsProject_dotNET\CampusEats\CampusEats.Frontend\Services\AuthorizationMessageHandler.cs
	namespace 	
CampusEatsFrontend
 
. 
Services %
;% &
public 
class '
AuthorizationMessageHandler (
:) *
DelegatingHandler+ <
{ 
private 
readonly 

IJSRuntime 

_jsRuntime  *
;* +
public

 
'
AuthorizationMessageHandler

 &
(

& '

IJSRuntime

' 1
	jsRuntime

2 ;
)

; <
{ 

_jsRuntime 
= 
	jsRuntime 
; 
} 
	protected 
override 
async 
Task !
<! "
HttpResponseMessage" 5
>5 6
	SendAsync7 @
(@ A
HttpRequestMessageA S
requestT [
,[ \
CancellationToken] n
cancellationToken	o Ä
)
Ä Å
{ 
try 
{ 	
var 
token 
= 
await 

_jsRuntime (
.( )
InvokeAsync) 4
<4 5
string5 ;
>; <
(< =
$str= S
,S T
$strU `
,` a
cancellationTokenb s
)s t
;t u
if 
( 
! 
string 
. 
IsNullOrEmpty %
(% &
token& +
)+ ,
), -
{ 
request 
. 
Headers 
.  
Authorization  -
=. /
new0 3%
AuthenticationHeaderValue4 M
(M N
$strN V
,V W
tokenX ]
)] ^
;^ _
} 
} 	
catch 
{ 	
} 	
return   
await   
base   
.   
	SendAsync   #
(  # $
request  $ +
,  + ,
cancellationToken  - >
)  > ?
;  ? @
}!! 
}"" ñb
|C:\Users\Veronica\Desktop\dot_Net\CampusEats\CampusEatsProject_dotNET\CampusEats\CampusEats.Frontend\Services\CartService.cs
	namespace 	
CampusEatsFrontend
 
. 
Services %
;% &
public		 
class		 
CartService		 
{

 
private 
readonly 
List 
< 
CartItem "
>" #
_items$ *
=+ ,
new- 0
(0 1
)1 2
;2 3
private 
readonly 

IJSRuntime 

_jsRuntime  *
;* +
private 
bool 
_initialized 
= 
false  %
;% &
public 

event 
Action 
? 
OnChange !
;! "
public 

CartService 
( 

IJSRuntime !
	jsRuntime" +
)+ ,
{ 

_jsRuntime 
= 
	jsRuntime 
; 
} 
public 

IReadOnlyList 
< 
CartItem !
>! "
Items# (
=>) +
_items, 2
.2 3

AsReadOnly3 =
(= >
)> ?
;? @
public 

int 

TotalItems 
=> 
_items #
.# $
Sum$ '
(' (
i( )
=>* ,
i- .
.. /
Quantity/ 7
)7 8
;8 9
public 

decimal 

TotalPrice 
=>  
_items! '
.' (
Sum( +
(+ ,
i, -
=>. 0
i1 2
.2 3
Subtotal3 ;
); <
;< =
public 

async 
Task 
InitializeAsync %
(% &
)& '
{ 
if 

( 
_initialized 
) 
return  
;  !
try   
{!! 	
var"" 
json"" 
="" 
await"" 

_jsRuntime"" '
.""' (
InvokeAsync""( 3
<""3 4
string""4 :
?"": ;
>""; <
(""< =
$str""= S
,""S T
$str""U [
)""[ \
;""\ ]
if## 
(## 
!## 
string## 
.## 
IsNullOrEmpty## %
(##% &
json##& *
)##* +
)##+ ,
{$$ 
var%% 
items%% 
=%% 
JsonSerializer%% *
.%%* +
Deserialize%%+ 6
<%%6 7
List%%7 ;
<%%; <
CartItem%%< D
>%%D E
>%%E F
(%%F G
json%%G K
)%%K L
;%%L M
if&& 
(&& 
items&& 
!=&& 
null&& !
)&&! "
{'' 
_items(( 
.(( 
Clear((  
(((  !
)((! "
;((" #
_items)) 
.)) 
AddRange)) #
())# $
items))$ )
)))) *
;))* +
}** 
}++ 
},, 	
catch-- 
{.. 	
}00 	
_initialized22 
=22 
true22 
;22 
NotifyStateChanged33 
(33 
)33 
;33 
}44 
public66 

async66 
Task66 
AddMenuAsync66 "
(66" #
MenuDto66# *
menu66+ /
,66/ 0
int661 4
quantity665 =
=66> ?
$num66@ A
)66A B
{77 
if88 

(88 
!88 
menu88 
.88 
Price88 
.88 
HasValue88  
||88! #
menu88$ (
.88( )
Price88) .
.88. /
Value88/ 4
<=885 7
$num888 9
)889 :
return99 
;99 
var;; 
existingItem;; 
=;; 
_items;; !
.;;! "
FirstOrDefault;;" 0
(;;0 1
i;;1 2
=>;;3 5
i;;6 7
.;;7 8
MenuId;;8 >
==;;? A
menu;;B F
.;;F G
Id;;G I
);;I J
;;;J K
if== 

(== 
existingItem== 
!=== 
null==  
)==  !
{>> 	
existingItem?? 
.?? 
Quantity?? !
+=??" $
quantity??% -
;??- .
}@@ 	
elseAA 
{BB 	
_itemsCC 
.CC 
AddCC 
(CC 
newCC 
CartItemCC #
{DD 
MenuIdEE 
=EE 
menuEE 
.EE 
IdEE  
,EE  !

MenuItemIdFF 
=FF 
nullFF !
,FF! "
NameGG 
=GG 
menuGG 
.GG 
NameGG  
,GG  !
PriceHH 
=HH 
menuHH 
.HH 
PriceHH "
.HH" #
ValueHH# (
,HH( )
QuantityII 
=II 
quantityII #
,II# $
ImageUrlJJ 
=JJ 
menuJJ 
.JJ  
ImageUrlJJ  (
}KK 
)KK 
;KK 
}LL 	
awaitNN #
SaveToLocalStorageAsyncNN %
(NN% &
)NN& '
;NN' (
NotifyStateChangedOO 
(OO 
)OO 
;OO 
}PP 
publicRR 

asyncRR 
TaskRR 
AddMenuItemAsyncRR &
(RR& '
MenuItemDtoRR' 2
menuItemRR3 ;
,RR; <
intRR= @
quantityRRA I
=RRJ K
$numRRL M
)RRM N
{SS 
ifTT 

(TT 
menuItemTT 
.TT 
PriceTT 
<=TT 
$numTT 
)TT  
returnUU 
;UU 
varWW 
existingItemWW 
=WW 
_itemsWW !
.WW! "
FirstOrDefaultWW" 0
(WW0 1
iWW1 2
=>WW3 5
iWW6 7
.WW7 8

MenuItemIdWW8 B
==WWC E
menuItemWWF N
.WWN O
IdWWO Q
)WWQ R
;WWR S
ifYY 

(YY 
existingItemYY 
!=YY 
nullYY  
)YY  !
{ZZ 	
existingItem[[ 
.[[ 
Quantity[[ !
+=[[" $
quantity[[% -
;[[- .
}\\ 	
else]] 
{^^ 	
_items__ 
.__ 
Add__ 
(__ 
new__ 
CartItem__ #
{`` 
MenuIdaa 
=aa 
nullaa 
,aa 

MenuItemIdbb 
=bb 
menuItembb %
.bb% &
Idbb& (
,bb( )
Namecc 
=cc 
menuItemcc 
.cc  
Namecc  $
,cc$ %
Pricedd 
=dd 
menuItemdd  
.dd  !
Pricedd! &
,dd& '
Quantityee 
=ee 
quantityee #
,ee# $
ImageUrlff 
=ff 
menuItemff #
.ff# $
ImageUrlff$ ,
}gg 
)gg 
;gg 
}hh 	
awaitjj #
SaveToLocalStorageAsyncjj %
(jj% &
)jj& '
;jj' (
NotifyStateChangedkk 
(kk 
)kk 
;kk 
}ll 
publicoo 

asyncoo 
Taskoo 
AddItemAsyncoo "
(oo" #
MenuDtooo# *
menuoo+ /
,oo/ 0
intoo1 4
quantityoo5 =
=oo> ?
$numoo@ A
)ooA B
{pp 
awaitqq 
AddMenuAsyncqq 
(qq 
menuqq 
,qq  
quantityqq! )
)qq) *
;qq* +
}rr 
publictt 

asynctt 
Tasktt 
RemoveItemAsynctt %
(tt% &
Guidtt& *
menuIdtt+ 1
)tt1 2
{uu 
_itemsvv 
.vv 
	RemoveAllvv 
(vv 
ivv 
=>vv 
ivv 
.vv  
MenuIdvv  &
==vv' )
menuIdvv* 0
)vv0 1
;vv1 2
awaitww #
SaveToLocalStorageAsyncww %
(ww% &
)ww& '
;ww' (
NotifyStateChangedxx 
(xx 
)xx 
;xx 
}yy 
public{{ 

async{{ 
Task{{ 
RemoveMenuItemAsync{{ )
({{) *
Guid{{* .

menuItemId{{/ 9
){{9 :
{|| 
_items}} 
.}} 
	RemoveAll}} 
(}} 
i}} 
=>}} 
i}} 
.}}  

MenuItemId}}  *
==}}+ -

menuItemId}}. 8
)}}8 9
;}}9 :
await~~ #
SaveToLocalStorageAsync~~ %
(~~% &
)~~& '
;~~' (
NotifyStateChanged 
( 
) 
; 
}
ÄÄ 
public
ÇÇ 

async
ÇÇ 
Task
ÇÇ !
UpdateQuantityAsync
ÇÇ )
(
ÇÇ) *
Guid
ÇÇ* .
id
ÇÇ/ 1
,
ÇÇ1 2
int
ÇÇ3 6
quantity
ÇÇ7 ?
,
ÇÇ? @
bool
ÇÇA E

isMenuItem
ÇÇF P
=
ÇÇQ R
false
ÇÇS X
)
ÇÇX Y
{
ÉÉ 
var
ÑÑ 
item
ÑÑ 
=
ÑÑ 

isMenuItem
ÑÑ 
?
ÖÖ 
_items
ÖÖ 
.
ÖÖ 
FirstOrDefault
ÖÖ #
(
ÖÖ# $
i
ÖÖ$ %
=>
ÖÖ& (
i
ÖÖ) *
.
ÖÖ* +

MenuItemId
ÖÖ+ 5
==
ÖÖ6 8
id
ÖÖ9 ;
)
ÖÖ; <
:
ÜÜ 
_items
ÜÜ 
.
ÜÜ 
FirstOrDefault
ÜÜ #
(
ÜÜ# $
i
ÜÜ$ %
=>
ÜÜ& (
i
ÜÜ) *
.
ÜÜ* +
MenuId
ÜÜ+ 1
==
ÜÜ2 4
id
ÜÜ5 7
)
ÜÜ7 8
;
ÜÜ8 9
if
àà 

(
àà 
item
àà 
!=
àà 
null
àà 
)
àà 
{
ââ 	
if
ää 
(
ää 
quantity
ää 
<=
ää 
$num
ää 
)
ää 
{
ãã 
if
åå 
(
åå 

isMenuItem
åå 
)
åå 
await
çç !
RemoveMenuItemAsync
çç -
(
çç- .
id
çç. 0
)
çç0 1
;
çç1 2
else
éé 
await
èè 
RemoveItemAsync
èè )
(
èè) *
id
èè* ,
)
èè, -
;
èè- .
}
êê 
else
ëë 
{
íí 
item
ìì 
.
ìì 
Quantity
ìì 
=
ìì 
quantity
ìì  (
;
ìì( )
await
îî %
SaveToLocalStorageAsync
îî -
(
îî- .
)
îî. /
;
îî/ 0 
NotifyStateChanged
ïï "
(
ïï" #
)
ïï# $
;
ïï$ %
}
ññ 
}
óó 	
}
òò 
public
öö 

async
öö 
Task
öö 

ClearAsync
öö  
(
öö  !
)
öö! "
{
õõ 
_items
úú 
.
úú 
Clear
úú 
(
úú 
)
úú 
;
úú 
await
ùù %
SaveToLocalStorageAsync
ùù %
(
ùù% &
)
ùù& '
;
ùù' ( 
NotifyStateChanged
ûû 
(
ûû 
)
ûû 
;
ûû 
}
üü 
private
°° 
async
°° 
Task
°° %
SaveToLocalStorageAsync
°° .
(
°°. /
)
°°/ 0
{
¢¢ 
try
££ 
{
§§ 	
var
•• 
json
•• 
=
•• 
JsonSerializer
•• %
.
••% &
	Serialize
••& /
(
••/ 0
_items
••0 6
)
••6 7
;
••7 8
await
¶¶ 

_jsRuntime
¶¶ 
.
¶¶ 
InvokeVoidAsync
¶¶ ,
(
¶¶, -
$str
¶¶- C
,
¶¶C D
$str
¶¶E K
,
¶¶K L
json
¶¶M Q
)
¶¶Q R
;
¶¶R S
}
ßß 	
catch
®® 
{
©© 	
}
´´ 	
}
¨¨ 
private
ÆÆ 
void
ÆÆ  
NotifyStateChanged
ÆÆ #
(
ÆÆ# $
)
ÆÆ$ %
=>
ÆÆ& (
OnChange
ÆÆ) 1
?
ÆÆ1 2
.
ÆÆ2 3
Invoke
ÆÆ3 9
(
ÆÆ9 :
)
ÆÆ: ;
;
ÆÆ; <
}ØØ åt
|C:\Users\Veronica\Desktop\dot_Net\CampusEats\CampusEatsProject_dotNET\CampusEats\CampusEats.Frontend\Services\AuthService.cs
	namespace 	
CampusEatsFrontend
 
. 
Services %
;% &
public		 
class		 
AuthService		 
{

 
private 
readonly 

HttpClient 
_http  %
;% &
private 
readonly 

IJSRuntime 
_js  #
;# $
private 
readonly !
JsonSerializerOptions *
_jsonOptions+ 7
;7 8
private 
UserDto 
? 
_currentUser !
;! "
public 

AuthService 
( 

HttpClient !
http" &
,& '

IJSRuntime( 2
js3 5
)5 6
{ 
_http 
= 
http 
; 
_js 
= 
js 
; 
_jsonOptions 
= 
new !
JsonSerializerOptions 0
{ 	'
PropertyNameCaseInsensitive '
=( )
true* .
,. /

Converters 
= 
{ 
new #
JsonStringEnumConverter 6
(6 7
)7 8
}9 :
} 	
;	 

} 
public 

UserDto 
? 
CurrentUser 
=>  "
_currentUser# /
;/ 0
public 

bool 
IsAuthenticated 
=>  "
_currentUser# /
!=0 2
null3 7
;7 8
public 

async 
Task 
< 
bool 
> 

LoginAsync &
(& '
LoginRequest' 3
request4 ;
); <
{ 
try   
{!! 	
var"" 
response"" 
="" 
await""  
_http""! &
.""& '
PostAsJsonAsync""' 6
(""6 7
$str""7 G
,""G H
request""I P
,""P Q
_jsonOptions""R ^
)""^ _
;""_ `
if$$ 
($$ 
!$$ 
response$$ 
.$$ 
IsSuccessStatusCode$$ -
)$$- .
return%% 
false%% 
;%% 
var'' 
result'' 
='' 
await'' 
response'' '
.''' (
Content''( /
.''/ 0
ReadFromJsonAsync''0 A
<''A B
LoginResponse''B O
>''O P
(''P Q
_jsonOptions''Q ]
)''] ^
;''^ _
if(( 
((( 
result(( 
==(( 
null(( 
)(( 
return)) 
false)) 
;)) 
_currentUser++ 
=++ 
new++ 
UserDto++ &
{,, 
UserId-- 
=-- 
result-- 
.--  
UserId--  &
,--& '
Username.. 
=.. 
result.. !
...! "
Username.." *
,..* +
Role// 
=// 
result// 
.// 
Role// "
,//" #
Token00 
=00 
result00 
.00 
Token00 $
}11 
;11 
const33 
string33 

identifier33 #
=33$ %
$str33& <
;33< =
await55 
_js55 
.55 
InvokeVoidAsync55 %
(55% &

identifier55& 0
,550 1
$str552 =
,55= >
result55? E
.55E F
Token55F K
)55K L
;55L M
await66 
_js66 
.66 
InvokeVoidAsync66 %
(66% &

identifier66& 0
,660 1
$str662 :
,66: ;
result66< B
.66B C
UserId66C I
.66I J
ToString66J R
(66R S
)66S T
)66T U
;66U V
await77 
_js77 
.77 
InvokeVoidAsync77 %
(77% &

identifier77& 0
,770 1
$str772 <
,77< =
result77> D
.77D E
Username77E M
)77M N
;77N O
await88 
_js88 
.88 
InvokeVoidAsync88 %
(88% &

identifier88& 0
,880 1
$str882 8
,888 9
result88: @
.88@ A
Role88A E
)88E F
;88F G
_http;; 
.;; !
DefaultRequestHeaders;; '
.;;' (
Authorization;;( 5
=;;6 7
new<< 
System<< 
.<< 
Net<< 
.<< 
Http<< #
.<<# $
Headers<<$ +
.<<+ ,%
AuthenticationHeaderValue<<, E
(<<E F
$str<<F N
,<<N O
result<<P V
.<<V W
Token<<W \
)<<\ ]
;<<] ^
return>> 
true>> 
;>> 
}?? 	
catch@@ 
{AA 	
returnBB 
falseBB 
;BB 
}CC 	
}DD 
publicFF 

asyncFF 
TaskFF 
<FF 
RegisterResultFF $
>FF$ %
RegisterAsyncFF& 3
(FF3 4
RegisterRequestFF4 C
requestFFD K
)FFK L
{GG 
tryHH 
{II 	
varJJ 
responseJJ 
=JJ 
awaitJJ  
_httpJJ! &
.JJ& '
PostAsJsonAsyncJJ' 6
(JJ6 7
$strJJ7 J
,JJJ K
requestJJL S
,JJS T
_jsonOptionsJJU a
)JJa b
;JJb c
ifLL 
(LL 
responseLL 
.LL 
IsSuccessStatusCodeLL ,
)LL, -
{MM 
returnNN 
newNN 
RegisterResultNN )
{NN* +
SuccessNN, 3
=NN4 5
trueNN6 :
}NN; <
;NN< =
}OO 
varQQ 
errorContentQQ 
=QQ 
awaitQQ $
responseQQ% -
.QQ- .
ContentQQ. 5
.QQ5 6
ReadAsStringAsyncQQ6 G
(QQG H
)QQH I
;QQI J
returnRR 
newRR 
RegisterResultRR %
{SS 
SuccessTT 
=TT 
falseTT 
,TT  
ErrorMessageUU 
=UU 
$"UU !
$strUU! )
{UU) *
responseUU* 2
.UU2 3

StatusCodeUU3 =
}UU= >
$strUU> G
{UUG H
errorContentUUH T
}UUT U
"UUU V
}VV 
;VV 
}WW 	
catchXX 
(XX 
	ExceptionXX 
exXX 
)XX 
{YY 	
returnZZ 
newZZ 
RegisterResultZZ %
{[[ 
Success\\ 
=\\ 
false\\ 
,\\  
ErrorMessage]] 
=]] 
$"]] !
$str]]! ,
{]], -
ex]]- /
.]]/ 0
Message]]0 7
}]]7 8
"]]8 9
}^^ 
;^^ 
}__ 	
}`` 
publicbb 

asyncbb 
Taskbb 
LogoutAsyncbb !
(bb! "
)bb" #
{cc 
trydd 
{ee 	
ifgg 
(gg 
IsAuthenticatedgg 
)gg  
{hh 
awaitii 
_httpii 
.ii 
	PostAsyncii %
(ii% &
$strii& 7
,ii7 8
nullii9 =
)ii= >
;ii> ?
}jj 
}kk 	
finallyll 
{mm 	
awaitoo 
_jsoo 
.oo 
InvokeVoidAsyncoo %
(oo% &
$stroo& ?
,oo? @
$strooA L
)ooL M
;ooM N
awaitpp 
_jspp 
.pp 
InvokeVoidAsyncpp %
(pp% &
$strpp& ?
,pp? @
$strppA I
)ppI J
;ppJ K
awaitqq 
_jsqq 
.qq 
InvokeVoidAsyncqq %
(qq% &
$strqq& ?
,qq? @
$strqqA K
)qqK L
;qqL M
awaitrr 
_jsrr 
.rr 
InvokeVoidAsyncrr %
(rr% &
$strrr& ?
,rr? @
$strrrA G
)rrG H
;rrH I
_currentUsertt 
=tt 
nulltt 
;tt  
_httpuu 
.uu !
DefaultRequestHeadersuu '
.uu' (
Authorizationuu( 5
=uu6 7
nulluu8 <
;uu< =
}vv 	
}ww 
publicyy 

asyncyy 
Taskyy 
<yy 
(yy 
boolyy 
successyy #
,yy# $
stringyy% +
?yy+ ,
errorMessageyy- 9
)yy9 :
>yy: ;
ChangePasswordAsyncyy< O
(yyO P!
ChangePasswordRequestyyP e
requestyyf m
)yym n
{zz 
try{{ 
{|| 	
var}} 
response}} 
=}} 
await}}  
_http}}! &
.}}& '
PostAsJsonAsync}}' 6
(}}6 7
$str}}7 Q
,}}Q R
request}}S Z
,}}Z [
_jsonOptions}}\ h
)}}h i
;}}i j
if 
( 
response 
. 
IsSuccessStatusCode ,
), -
{
ÄÄ 
return
ÅÅ 
(
ÅÅ 
true
ÅÅ 
,
ÅÅ 
null
ÅÅ "
)
ÅÅ" #
;
ÅÅ# $
}
ÇÇ 
var
ÖÖ 
errorContent
ÖÖ 
=
ÖÖ 
await
ÖÖ $
response
ÖÖ% -
.
ÖÖ- .
Content
ÖÖ. 5
.
ÖÖ5 6
ReadAsStringAsync
ÖÖ6 G
(
ÖÖG H
)
ÖÖH I
;
ÖÖI J
return
ÜÜ 
(
ÜÜ 
false
ÜÜ 
,
ÜÜ 
string
ÜÜ !
.
ÜÜ! "
IsNullOrEmpty
ÜÜ" /
(
ÜÜ/ 0
errorContent
ÜÜ0 <
)
ÜÜ< =
?
ÜÜ> ?
$str
ÜÜ@ [
:
ÜÜ\ ]
errorContent
ÜÜ^ j
)
ÜÜj k
;
ÜÜk l
}
áá 	
catch
àà 
(
àà 
	Exception
àà 
ex
àà 
)
àà 
{
ââ 	
return
ää 
(
ää 
false
ää 
,
ää 
ex
ää 
.
ää 
Message
ää %
)
ää% &
;
ää& '
}
ãã 	
}
åå 
public
éé 

async
éé 
Task
éé 
InitializeAsync
éé %
(
éé% &
)
éé& '
{
èè 
try
êê 
{
ëë 	
var
íí 
token
íí 
=
íí 
await
íí 
_js
íí !
.
íí! "
InvokeAsync
íí" -
<
íí- .
string
íí. 4
?
íí4 5
>
íí5 6
(
íí6 7
$str
íí7 M
,
ííM N
$str
ííO Z
)
ííZ [
;
íí[ \
if
îî 
(
îî 
string
îî 
.
îî 
IsNullOrEmpty
îî $
(
îî$ %
token
îî% *
)
îî* +
)
îî+ ,
return
ïï 
;
ïï 
var
óó 
	userIdStr
óó 
=
óó 
await
óó !
_js
óó" %
.
óó% &
InvokeAsync
óó& 1
<
óó1 2
string
óó2 8
?
óó8 9
>
óó9 :
(
óó: ;
$str
óó; Q
,
óóQ R
$str
óóS [
)
óó[ \
;
óó\ ]
var
òò 
username
òò 
=
òò 
await
òò  
_js
òò! $
.
òò$ %
InvokeAsync
òò% 0
<
òò0 1
string
òò1 7
?
òò7 8
>
òò8 9
(
òò9 :
$str
òò: P
,
òòP Q
$str
òòR \
)
òò\ ]
;
òò] ^
var
ôô 
role
ôô 
=
ôô 
await
ôô 
_js
ôô  
.
ôô  !
InvokeAsync
ôô! ,
<
ôô, -
string
ôô- 3
?
ôô3 4
>
ôô4 5
(
ôô5 6
$str
ôô6 L
,
ôôL M
$str
ôôN T
)
ôôT U
;
ôôU V
if
õõ 
(
õõ 
!
õõ 
string
õõ 
.
õõ 
IsNullOrEmpty
õõ %
(
õõ% &
	userIdStr
õõ& /
)
õõ/ 0
&&
õõ1 3
Guid
õõ4 8
.
õõ8 9
TryParse
õõ9 A
(
õõA B
	userIdStr
õõB K
,
õõK L
out
õõM P
var
õõQ T
userId
õõU [
)
õõ[ \
)
õõ\ ]
{
úú 
_currentUser
ùù 
=
ùù 
new
ùù "
UserDto
ùù# *
{
ûû 
UserId
üü 
=
üü 
userId
üü #
,
üü# $
Username
†† 
=
†† 
username
†† '
??
††( *
$str
††+ -
,
††- .
Role
°° 
=
°° 
role
°° 
??
°°  "
$str
°°# +
,
°°+ ,
Token
¢¢ 
=
¢¢ 
token
¢¢ !
}
££ 
;
££ 
_http
•• 
.
•• #
DefaultRequestHeaders
•• +
.
••+ ,
Authorization
••, 9
=
••: ;
new
¶¶ 
System
¶¶ 
.
¶¶ 
Net
¶¶ "
.
¶¶" #
Http
¶¶# '
.
¶¶' (
Headers
¶¶( /
.
¶¶/ 0'
AuthenticationHeaderValue
¶¶0 I
(
¶¶I J
$str
¶¶J R
,
¶¶R S
token
¶¶T Y
)
¶¶Y Z
;
¶¶Z [
}
ßß 
}
®® 	
catch
©© 
{
™™ 	
}
¨¨ 	
}
≠≠ 
private
ØØ 
class
ØØ 
LoginResponse
ØØ 
{
∞∞ 
public
±± 
string
±± 
Token
±± 
{
±± 
get
±± !
;
±±! "
set
±±# &
;
±±& '
}
±±( )
=
±±* +
string
±±, 2
.
±±2 3
Empty
±±3 8
;
±±8 9
public
≤≤ 
Guid
≤≤ 
UserId
≤≤ 
{
≤≤ 
get
≤≤  
;
≤≤  !
set
≤≤" %
;
≤≤% &
}
≤≤' (
public
≥≥ 
string
≥≥ 
Username
≥≥ 
{
≥≥  
get
≥≥! $
;
≥≥$ %
set
≥≥& )
;
≥≥) *
}
≥≥+ ,
=
≥≥- .
string
≥≥/ 5
.
≥≥5 6
Empty
≥≥6 ;
;
≥≥; <
public
¥¥ 
string
¥¥ 
Role
¥¥ 
{
¥¥ 
get
¥¥  
;
¥¥  !
set
¥¥" %
;
¥¥% &
}
¥¥' (
=
¥¥) *
string
¥¥+ 1
.
¥¥1 2
Empty
¥¥2 7
;
¥¥7 8
}
µµ 
}∂∂ Ë

ÑC:\Users\Veronica\Desktop\dot_Net\CampusEats\CampusEatsProject_dotNET\CampusEats\CampusEats.Frontend\Models\User\ClientProfileDto.cs
	namespace 	
CampusEatsFrontend
 
. 
Models #
.# $
User$ (
;( )
public 
class 
ClientProfileDto 
{ 
public 

Guid 
Id 
{ 
get 
; 
set 
; 
}  
public 

string 
Username 
{ 
get  
;  !
set" %
;% &
}' (
=) *
string+ 1
.1 2
Empty2 7
;7 8
public 

string 
Email 
{ 
get 
; 
set "
;" #
}$ %
=& '
string( .
.. /
Empty/ 4
;4 5
public 

string 
Role 
{ 
get 
; 
set !
;! "
}# $
=% &
string' -
.- .
Empty. 3
;3 4
public		 

DateTime		 
	CreatedAt		 
{		 
get		  #
;		# $
set		% (
;		( )
}		* +
}

 ß
ÑC:\Users\Veronica\Desktop\dot_Net\CampusEats\CampusEatsProject_dotNET\CampusEats\CampusEats.Frontend\Models\Payment\PaymentModels.cs
	namespace 	
CampusEatsFrontend
 
. 
Models #
.# $
Payment$ +
;+ ,
public 
enum 
PaymentStatus 
{ 
Pending 
, 
	Succeeded 
, 
Failed 

} 
public

 
enum

 
PaymentMethod

 
{ 
MockCard 
, 

StripeTest 
} 
public 
class 

PaymentDto 
{ 
public 

Guid 
Id 
{ 
get 
; 
set 
; 
}  
public 

Guid 
UserId 
{ 
get 
; 
set !
;! "
}# $
public 

Guid 
OrderId 
{ 
get 
; 
set "
;" #
}$ %
public 

decimal 
Amount 
{ 
get 
;  
set! $
;$ %
}& '
public 

PaymentStatus 
Status 
{  !
get" %
;% &
set' *
;* +
}, -
public 

PaymentMethod 
Method 
{  !
get" %
;% &
set' *
;* +
}, -
public 

string 
? 
ExternalReference $
{% &
get' *
;* +
set, /
;/ 0
}1 2
public 

DateTime 
CreatedAtUtc  
{! "
get# &
;& '
set( +
;+ ,
}- .
} 
public 
class  
CreatePaymentRequest !
{ 
public 

Guid 
UserId 
{ 
get 
; 
set !
;! "
}# $
public 

Guid 
OrderId 
{ 
get 
; 
set "
;" #
}$ %
public   

decimal   
Amount   
{   
get   
;    
set  ! $
;  $ %
}  & '
public!! 

PaymentMethod!! 
Method!! 
{!!  !
get!!" %
;!!% &
set!!' *
;!!* +
}!!, -
public"" 

int"" 
?"" 
PointsToUse"" 
{"" 
get"" !
;""! "
set""# &
;""& '
}""( )
}## œ	
ôC:\Users\Veronica\Desktop\dot_Net\CampusEats\CampusEatsProject_dotNET\CampusEats\CampusEats.Frontend\Models\Payment\CreateStripeCheckoutSessionRequest.cs
	namespace 	
CampusEatsFrontend
 
. 
Models #
.# $
Payment$ +
;+ ,
public 
class .
"CreateStripeCheckoutSessionRequest /
{ 
public 

Guid 
UserId 
{ 
get 
; 
set !
;! "
}# $
public 

Guid 
OrderId 
{ 
get 
; 
set "
;" #
}$ %
public 

int 
? 
PointsToUse 
{ 
get !
;! "
set# &
;& '
}( )
} 
public

 
class

 )
StripeCheckoutSessionResponse

 *
{ 
public 

string 
CheckoutUrl 
{ 
get  #
;# $
set% (
;( )
}* +
=, -
string. 4
.4 5
Empty5 :
;: ;
} ∂
ÜC:\Users\Veronica\Desktop\dot_Net\CampusEats\CampusEatsProject_dotNET\CampusEats\CampusEats.Frontend\Models\Order\PlaceOrderRequest.cs
	namespace 	
CampusEatsFrontend
 
. 
Models #
.# $
Order$ )
;) *
public 
class 
PlaceOrderRequest 
{ 
public 

Guid 
ClientId 
{ 
get 
; 
set  #
;# $
}% &
public 

List 
< 
Guid 
> 
MenuIDs 
{ 
get  #
;# $
set% (
;( )
}* +
=, -
new. 1
(1 2
)2 3
;3 4
public 

List 
< 
Guid 
> 
ItemIDs 
{ 
get  #
;# $
set% (
;( )
}* +
=, -
new. 1
(1 2
)2 3
;3 4
} µ
ÄC:\Users\Veronica\Desktop\dot_Net\CampusEats\CampusEatsProject_dotNET\CampusEats\CampusEats.Frontend\Models\Order\OrderStatus.cs
	namespace 	
CampusEatsFrontend
 
. 
Models #
.# $
Order$ )
;) *
public 
enum 
OrderStatus 
{ 
Pending 
= 
$num 
, 
	Confirmed 
= 
$num 
, 
	Preparing 
= 
$num 
, 
	Completed 
= 
$num 
, 
	Cancelled		 
=		 
$num		 
}

 è
}C:\Users\Veronica\Desktop\dot_Net\CampusEats\CampusEatsProject_dotNET\CampusEats\CampusEats.Frontend\Models\Order\OrderDto.cs
	namespace 	
CampusEatsFrontend
 
. 
Models #
.# $
Order$ )
;) *
public 
class 
OrderDto 
{ 
public 

Guid 
Id 
{ 
get 
; 
set 
; 
}  
public 

Guid 
ClientId 
{ 
get 
; 
set  #
;# $
}% &
public 

decimal 
Price 
{ 
get 
; 
set  #
;# $
}% &
public 

List 
< 
Guid 
> 
MenuIDs 
{ 
get  #
;# $
set% (
;( )
}* +
=, -
new. 1
(1 2
)2 3
;3 4
public		 

List		 
<		 
Guid		 
>		 
ItemIDs		 
{		 
get		  #
;		# $
set		% (
;		( )
}		* +
=		, -
new		. 1
(		1 2
)		2 3
;		3 4
public

 

DateTime

 
	CreatedAt

 
{

 
get

  #
;

# $
set

% (
;

( )
}

* +
public 

OrderStatus 
Status 
{ 
get  #
;# $
set% (
;( )
}* +
} æ
ÑC:\Users\Veronica\Desktop\dot_Net\CampusEats\CampusEatsProject_dotNET\CampusEats\CampusEats.Frontend\Models\Order\OrderDetailsDto.cs
	namespace 	
CampusEatsFrontend
 
. 
Models #
.# $
Order$ )
;) *
public 
class 
OrderDetailsDto 
{ 
public 

Guid 
Id 
{ 
get 
; 
set 
; 
}  
public 

Guid 
ClientId 
{ 
get 
; 
set  #
;# $
}% &
public 

string 
ClientUsername  
{! "
get# &
;& '
set( +
;+ ,
}- .
=/ 0
string1 7
.7 8
Empty8 =
;= >
public 

string 
ClientEmail 
{ 
get  #
;# $
set% (
;( )
}* +
=, -
string. 4
.4 5
Empty5 :
;: ;
public		 

decimal		 
Price		 
{		 
get		 
;		 
set		  #
;		# $
}		% &
public

 

List

 
<

 
Guid

 
>

 
MenuIDs

 
{

 
get

  #
;

# $
set

% (
;

( )
}

* +
=

, -
new

. 1
(

1 2
)

2 3
;

3 4
public 

List 
< 
Guid 
> 
ItemIDs 
{ 
get  #
;# $
set% (
;( )
}* +
=, -
new. 1
(1 2
)2 3
;3 4
public 

DateTime 
	CreatedAt 
{ 
get  #
;# $
set% (
;( )
}* +
public 

OrderStatus 
Status 
{ 
get  #
;# $
set% (
;( )
}* +
public 

List 
< 
OrderMenuItemDto  
>  !
Menus" '
{( )
get* -
;- .
set/ 2
;2 3
}4 5
=6 7
new8 ;
(; <
)< =
;= >
public 

List 
< 
OrderMenuItemDto  
>  !
Items" '
{( )
get* -
;- .
set/ 2
;2 3
}4 5
=6 7
new8 ;
(; <
)< =
;= >
} 
public 
class 
OrderMenuItemDto 
{ 
public 

Guid 
Id 
{ 
get 
; 
set 
; 
}  
public 

string 
Name 
{ 
get 
; 
set !
;! "
}# $
=% &
string' -
.- .
Empty. 3
;3 4
public 

decimal 
Price 
{ 
get 
; 
set  #
;# $
}% &
} ˙
ÖC:\Users\Veronica\Desktop\dot_Net\CampusEats\CampusEatsProject_dotNET\CampusEats\CampusEats.Frontend\Models\Menu\UpdateMenuRequest.cs
	namespace 	
CampusEatsFrontend
 
. 
Models #
.# $
Menu$ (
;( )
public 
class 
UpdateMenuRequest 
{ 
public 

Guid 
Id 
{ 
get 
; 
set 
; 
}  
public 

string 
Name 
{ 
get 
; 
set !
;! "
}# $
=% &
string' -
.- .
Empty. 3
;3 4
public 

decimal 
? 
Price 
{ 
get 
;  
set! $
;$ %
}& '
public 

List 
< 
Guid 
> 
ItemIds 
{ 
get  #
;# $
set% (
;( )
}* +
=, -
new. 1
(1 2
)2 3
;3 4
public		 

MenuCategory		 
Category		  
{		! "
get		# &
;		& '
set		( +
;		+ ,
}		- .
public

 

string

 
?

 
ImageUrl

 
{

 
get

 !
;

! "
set

# &
;

& '
}

( )
} Ï
{C:\Users\Veronica\Desktop\dot_Net\CampusEats\CampusEatsProject_dotNET\CampusEats\CampusEats.Frontend\Models\Menu\MenuDTO.cs
	namespace 	
CampusEatsFrontend
 
. 
Models #
.# $
Menu$ (
;( )
public 
class 
MenuDto 
{ 
public 

Guid 
Id 
{ 
get 
; 
set 
; 
}  
public 

string 
Name 
{ 
get 
; 
set !
;! "
}# $
=% &
null' +
!+ ,
;, -
public 

decimal 
? 
Price 
{ 
get 
;  
set! $
;$ %
}& '
public 

List 
< 
Guid 
> 
ItemIds 
{ 
get  #
;# $
set% (
;( )
}* +
=, -
[. /
]/ 0
;0 1
public		 

MenuCategory		 
Category		  
{		! "
get		# &
;		& '
set		( +
;		+ ,
}		- .
public

 

DietaryRestrictions

 
Restrictions

 +
{

, -
get

. 1
;

1 2
set

3 6
;

6 7
}

8 9
public 

string 
? 
ImageUrl 
{ 
get !
;! "
set# &
;& '
}( )
} ñ
ÄC:\Users\Veronica\Desktop\dot_Net\CampusEats\CampusEatsProject_dotNET\CampusEats\CampusEats.Frontend\Models\Menu\MenuCategory.cs
	namespace 	
CampusEatsFrontend
 
. 
Models #
.# $
Menu$ (
;( )
public 
enum 
MenuCategory 
{ 

Vegetarian 
, 
Vegan 	
,	 

Meat 
, 	
Seafood 
, 
Dessert		 
,		 
Asian

 	
,

	 

Mexican 
, 
Mediterranean 
, 
French 

,
 
Baltic 

,
 
Balkan 

,
 
Turkish 
, 
Traditional 
, 
	Breakfast 
, 
Lunch 	
,	 

Dinner 

} ’
áC:\Users\Veronica\Desktop\dot_Net\CampusEats\CampusEatsProject_dotNET\CampusEats\CampusEats.Frontend\Models\Menu\DietaryRestrictions.cs
	namespace 	
CampusEatsFrontend
 
. 
Models #
.# $
Menu$ (
;( )
[ 
Flags 
] 
public 
enum 
DietaryRestrictions 
{ 
FoodAllergyFriendly 
= 
$num 
, 
LactoseFree 
= 
$num 
, 

GlutenFree 
= 
$num 
, 
NutFree		 
=		 
$num		 
,		 
	DairyFree

 
=

 
$num

 
,

 
	NoSeafood 
= 
$num 
, 
} œ

ÖC:\Users\Veronica\Desktop\dot_Net\CampusEats\CampusEatsProject_dotNET\CampusEats\CampusEats.Frontend\Models\Menu\CreateMenuRequest.cs
	namespace 	
CampusEatsFrontend
 
. 
Models #
.# $
Menu$ (
;( )
public 
class 
CreateMenuRequest 
{ 
public 

string 
Name 
{ 
get 
; 
set !
;! "
}# $
=% &
null' +
!+ ,
;, -
public 

decimal 
? 
Price 
{ 
get 
;  
set! $
;$ %
}& '
public 

List 
< 
Guid 
> 
? 
ItemIds 
{  
get! $
;$ %
set& )
;) *
}+ ,
=- .
[/ 0
]0 1
;1 2
public 

MenuCategory 
Category  
{! "
get# &
;& '
set( +
;+ ,
}- .
public		 

string		 
?		 
ImageUrl		 
{		 
get		 !
;		! "
set		# &
;		& '
}		( )
}

 ü

çC:\Users\Veronica\Desktop\dot_Net\CampusEats\CampusEatsProject_dotNET\CampusEats\CampusEats.Frontend\Models\MenuItem\UpdateMenuItemRequest.cs
	namespace 	
CampusEatsFrontend
 
. 
Models #
.# $
MenuItem$ ,
;, -
public 
class !
UpdateMenuItemRequest "
{ 
public 

Guid 
Id 
{ 
get 
; 
set 
; 
}  
public 

string 
Name 
{ 
get 
; 
set !
;! "
}# $
=% &
string' -
.- .
Empty. 3
;3 4
public 

decimal 
Price 
{ 
get 
; 
set  #
;# $
}% &
public 

string 
? 
ImageUrl 
{ 
get !
;! "
set# &
;& '
}( )
public		 

List		 
<		 
string		 
>		 
?		 
	Allergens		 "
{		# $
get		% (
;		( )
set		* -
;		- .
}		/ 0
}

 ã

ÉC:\Users\Veronica\Desktop\dot_Net\CampusEats\CampusEatsProject_dotNET\CampusEats\CampusEats.Frontend\Models\MenuItem\MenuItemDto.cs
	namespace 	
CampusEatsFrontend
 
. 
Models #
.# $
MenuItem$ ,
;, -
public 
class 
MenuItemDto 
{ 
public 

Guid 
Id 
{ 
get 
; 
set 
; 
}  
public 

string 
Name 
{ 
get 
; 
set !
;! "
}# $
=% &
string' -
.- .
Empty. 3
;3 4
public 

decimal 
Price 
{ 
get 
; 
set  #
;# $
}% &
public 

string 
? 
ImageUrl 
{ 
get !
;! "
set# &
;& '
}( )
public		 

List		 
<		 
string		 
>		 
?		 
	Allergens		 "
{		# $
get		% (
;		( )
set		* -
;		- .
}		/ 0
}

 ã	
çC:\Users\Veronica\Desktop\dot_Net\CampusEats\CampusEatsProject_dotNET\CampusEats\CampusEats.Frontend\Models\MenuItem\CreateMenuItemRequest.cs
	namespace 	
CampusEatsFrontend
 
. 
Models #
.# $
MenuItem$ ,
;, -
public 
class !
CreateMenuItemRequest "
{ 
public 

string 
Name 
{ 
get 
; 
set !
;! "
}# $
=% &
string' -
.- .
Empty. 3
;3 4
public 

decimal 
Price 
{ 
get 
; 
set  #
;# $
}% &
public 

string 
? 
ImageUrl 
{ 
get !
;! "
set# &
;& '
}( )
public 

List 
< 
string 
> 
? 
	Allergens "
{# $
get% (
;( )
set* -
;- .
}/ 0
}		 ¿
ÑC:\Users\Veronica\Desktop\dot_Net\CampusEats\CampusEatsProject_dotNET\CampusEats\CampusEats.Frontend\Models\Loyalty\LoyaltyModels.cs
	namespace 	
CampusEatsFrontend
 
. 
Models #
.# $
Loyalty$ +
;+ ,
public 
enum "
LoyaltyTransactionType "
{ 
Earn 
, 	
Redeem 

,
 
CashbackBonus 
} 
public

 
enum

 
LoyaltyTier

 
{ 
Bronze 

,
 
Silver 

,
 
Gold 
, 	
Platinum 
, 
VIP 
} 
public 
class 
LoyaltyBalanceDto 
{ 
public 

Guid 
UserId 
{ 
get 
; 
set !
;! "
}# $
public 

int 
Points 
{ 
get 
; 
set  
;  !
}" #
public 

int 
TotalPointsEarned  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

LoyaltyTier 
CurrentTier "
{# $
get% (
;( )
set* -
;- .
}/ 0
public 

decimal 
CashbackRate 
{  !
get" %
;% &
set' *
;* +
}, -
public 

LoyaltyTier 
? 
NextTier  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

int 
PointsToNextTier 
{  !
get" %
;% &
set' *
;* +
}, -
} 
public 
class !
LoyaltyTransactionDto "
{ 
public   

Guid   
Id   
{   
get   
;   
set   
;   
}    
public!! 

Guid!! 
UserId!! 
{!! 
get!! 
;!! 
set!! !
;!!! "
}!!# $
public"" 
"
LoyaltyTransactionType"" !
Type""" &
{""' (
get"") ,
;"", -
set"". 1
;""1 2
}""3 4
public## 

int## 
Points## 
{## 
get## 
;## 
set##  
;##  !
}##" #
public$$ 

string$$ 
?$$ 
Description$$ 
{$$  
get$$! $
;$$$ %
set$$& )
;$$) *
}$$+ ,
public%% 

DateTime%% 
CreatedAtUtc%%  
{%%! "
get%%# &
;%%& '
set%%( +
;%%+ ,
}%%- .
}&& 
public(( 
class(( 
RedeemPointsRequest((  
{)) 
public** 

Guid** 
UserId** 
{** 
get** 
;** 
set** !
;**! "
}**# $
public++ 

int++ 
PointsToRedeem++ 
{++ 
get++  #
;++# $
set++% (
;++( )
}++* +
},, ˆ
|C:\Users\Veronica\Desktop\dot_Net\CampusEats\CampusEatsProject_dotNET\CampusEats\CampusEats.Frontend\Models\Cart\CartItem.cs
	namespace 	
CampusEatsFrontend
 
. 
Models #
.# $
Cart$ (
;( )
public 
class 
CartItem 
{ 
public 

Guid 
? 
MenuId 
{ 
get 
; 
set "
;" #
}$ %
public 

Guid 
? 

MenuItemId 
{ 
get !
;! "
set# &
;& '
}( )
public 

string 
Name 
{ 
get 
; 
set !
;! "
}# $
=% &
string' -
.- .
Empty. 3
;3 4
public 

decimal 
? 
Price 
{ 
get 
;  
set! $
;$ %
}& '
public		 

int		 
Quantity		 
{		 
get		 
;		 
set		 "
;		" #
}		$ %
public

 

string

 
?

 
ImageUrl

 
{

 
get

 !
;

! "
set

# &
;

& '
}

( )
public 

decimal 
Subtotal 
=> 
(  
Price  %
??& (
$num) *
)* +
*, -
Quantity. 6
;6 7
public 

bool 

IsMenuItem 
=> 

MenuItemId (
.( )
HasValue) 1
;1 2
} ∫	
{C:\Users\Veronica\Desktop\dot_Net\CampusEats\CampusEatsProject_dotNET\CampusEats\CampusEats.Frontend\Models\Auth\UserDto.cs
	namespace 	
CampusEatsFrontend
 
. 
Models #
.# $
Auth$ (
;( )
public 
class 
UserDto 
{ 
public 

Guid 
UserId 
{ 
get 
; 
set !
;! "
}# $
public 

string 
Username 
{ 
get  
;  !
set" %
;% &
}' (
=) *
string+ 1
.1 2
Empty2 7
;7 8
public 

string 
Role 
{ 
get 
; 
set !
;! "
}# $
=% &
string' -
.- .
Empty. 3
;3 4
public 

string 
Token 
{ 
get 
; 
set "
;" #
}$ %
=& '
string( .
.. /
Empty/ 4
;4 5
}		 ı
ÇC:\Users\Veronica\Desktop\dot_Net\CampusEats\CampusEatsProject_dotNET\CampusEats\CampusEats.Frontend\Models\Auth\RegisterResult.cs
	namespace 	
CampusEatsFrontend
 
. 
Models #
.# $
Auth$ (
;( )
public 
class 
RegisterResult 
{ 
public 

bool 
Success 
{ 
get 
; 
set "
;" #
}$ %
public 

string 
ErrorMessage 
{  
get! $
;$ %
set& )
;) *
}+ ,
=- .
string/ 5
.5 6
Empty6 ;
;; <
} Æ

ÉC:\Users\Veronica\Desktop\dot_Net\CampusEats\CampusEatsProject_dotNET\CampusEats\CampusEats.Frontend\Models\Auth\RegisterRequest.cs
	namespace 	
CampusEatsFrontend
 
. 
Models #
.# $
Auth$ (
;( )
public 
class 
RegisterRequest 
{ 
public 

string 
Username 
{ 
get  
;  !
set" %
;% &
}' (
=) *
string+ 1
.1 2
Empty2 7
;7 8
public 

string 
Email 
{ 
get 
; 
set "
;" #
}$ %
=& '
string( .
.. /
Empty/ 4
;4 5
public 

string 
Password 
{ 
get  
;  !
set" %
;% &
}' (
=) *
string+ 1
.1 2
Empty2 7
;7 8
public 

string 
ConfirmPassword !
{" #
get$ '
;' (
set) ,
;, -
}. /
=0 1
string2 8
.8 9
Empty9 >
;> ?
}		 ¡
ÄC:\Users\Veronica\Desktop\dot_Net\CampusEats\CampusEatsProject_dotNET\CampusEats\CampusEats.Frontend\Models\Auth\LoginRequest.cs
	namespace 	
CampusEatsFrontend
 
. 
Models #
.# $
Auth$ (
;( )
public 
class 
LoginRequest 
{ 
public 

string 
Email 
{ 
get 
; 
set "
;" #
}$ %
=& '
string( .
.. /
Empty/ 4
;4 5
public 

string 
Password 
{ 
get  
;  !
set" %
;% &
}' (
=) *
string+ 1
.1 2
Empty2 7
;7 8
} ¯
âC:\Users\Veronica\Desktop\dot_Net\CampusEats\CampusEatsProject_dotNET\CampusEats\CampusEats.Frontend\Models\Auth\ChangePasswordRequest.cs
	namespace 	
CampusEatsFrontend
 
. 
Models #
.# $
Auth$ (
;( )
public 
class !
ChangePasswordRequest "
{ 
public 

Guid 
UserId 
{ 
get 
; 
set !
;! "
}# $
public 

string 
CurrentPassword !
{" #
get$ '
;' (
set) ,
;, -
}. /
=0 1
string2 8
.8 9
Empty9 >
;> ?
public 

string 
NewPassword 
{ 
get  #
;# $
set% (
;( )
}* +
=, -
string. 4
.4 5
Empty5 :
;: ;
} 