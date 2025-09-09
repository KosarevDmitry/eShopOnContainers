start to sort out with deploy-all.ps1

D-src-sharp_eshopcontainer-eShopOnContainers-deploy-k8s-All-content
and 
D-src-sharp_eshopcontainer-eShopOnContainers-deploy-k8s-.--.tpl$-.--.yaml$-.--.ps1$-.--.md$-.--.txt$-content.cs_
contains my comments

there is no descriptions of params in deploy-all.ps1, but there is  in `deploy-all.sh`. look there
https://github.com/dotnet-architecture/eShopOnContainers/wiki/Deploy-to-Local-Kubernetes
recommend to run with args `.\deploy-all.ps1 -imageTag linux-latest -useLocalk8s $true -imagePullPolicy Never` 

// How to handle  the installation

1. firstly install create-aks.ps1  // :: need to find args description
2. enable-tls.ps1 , it  install cert-manager
next I think  needs to install certification` of https://letsencrypt.org/
 `tls-support`  helm chart  is not runned by  deploy-all.ps1 or some other.
https://letsencrypt.org/ provides free `tls sertification`

:+1: read wiki 
Question
---
dashboard-adminuser.yaml
helm-rbac.yaml
\aks-httpaddon-cfg.yaml
\nginx-ingress\
\nodeports\

ApiGateways ?
envoy
---

3. install infrastruture - mongo, sql, rabbitmq and two redis db
packages are "nosql-data","sql-data", "rabbitmq","keystore-data",  "basket-data"

cd ./helm

```powershell

$appName="eshop" 

$ingressValuesFile = "ingress_values.yaml"
$aksName
$aksRg
$dns = $(az aks show -n $aksName  -g $aksRg --query addonProfiles.httpApplicationRouting.config.HTTPApplicationRoutingZoneName)
$dns = $dns -replace '[\"]'
$dns = "$appName.$dns"
// note here are several value files

//image `mongo: 3.6.5-jessie`
$infra = "nosql-data" //name package name

//"$appName-$infra" is  release.Name arg

helm install "$appName-$infra" --values app.yaml --values inf.yaml --values $ingressValuesFile --set app.name=$appName --set inf.k8s.dns=$dns --set "ingress.hosts={$dns}" $infra     

// image `mcr.microsoft.com/mssql/server: 2019-latest`
$infra = "sql-data"
helm install "$appName-$infra" --values app.yaml --values inf.yaml --values $ingressValuesFile --set app.name=$appName --set inf.k8s.dns=$dns --set "ingress.hosts={$dns}" $infra     

//image  `rabbitmq: 3-management`
$infra = "rabbitmq"
helm install "$appName-$infra" --values app.yaml --values inf.yaml --values $ingressValuesFile --set app.name=$appName --set inf.k8s.dns=$dns --set "ingress.hosts={$dns}" $infra     

// two redis containers, image is the same
// used by Identity API
//image `redis:4.0.10` port 6379
$infra = "keystore-data"
helm install "$appName-$infra" --values app.yaml --values inf.yaml --values $ingressValuesFile --set app.name=$appName --set inf.k8s.dns=$dns --set "ingress.hosts={$dns}" $infra     

// used by basket-data service
//image `redis:4.0.10` port 6379
$infra = "basket-data"
helm install "$appName-$infra" --values app.yaml --values inf.yaml --values $ingressValuesFile --set app.name=$appName --set inf.k8s.dns=$dns --set "ingress.hosts={$dns}" $infra     

```
for registry create imagePullSecrets : explanation https://stackoverflow.com/questions/54354981/what-are-the-different-values-of-imagepullsecrets-and-what-does-regsecret-dowhy 

4.  install service 
4.1 first need to create and push images  by `docker-compose build` in \src
4.2 run deployment ps1.  specific notice: there is chain:
  env -> configmap.yaml -> values inf.yaml.
  value is simple representation of information then configmap is more complex, env is result. configmap is part of deployment.
  project code  looks to env.
`.\deploy-all.ps1 -imageTag linux-latest -useLocalk8s $true -imagePullPolicy Never` 

**@linkerd** it is proxy that installed to kubernetes
https://github.com/linkerd/linkerd2/tree/main

kubectl get deployment
-ingress-nginx https://kubernetes.github.io/ingress-nginx/


