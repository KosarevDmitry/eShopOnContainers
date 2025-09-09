install ingress
kubectl apply -f .\local-cm.yaml
.\deploy-all.ps1 -imageTag linux-latest -useLocalk8s $true -imagePullPolicy Never