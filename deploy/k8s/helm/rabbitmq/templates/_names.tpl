
{{- define "mongo-name" -}}
{{- if .Values.inf.rabbitmq.host -}}
{{- .Values.inf.rabbitmq.host -}}
{{- else -}}
{{- printf "%s" "rabbitmq" -}}
{{- end -}}
{{- end -}}
