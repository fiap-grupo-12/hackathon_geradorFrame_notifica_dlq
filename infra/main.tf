provider "aws" {
  region = "sa-east-1"
}

terraform {
  backend "s3" {
    bucket = "tfstate-grupo12-fiap-2025"
    key    = "lambda_geradorFrame_notifica/terraform.tfstate"
    region = "sa-east-1"
  }
}

# Definir a fila SQS
data "aws_sqs_queue" "sqs_processar_arquivo_dlq" {
  name = "sqs_processar_arquivo_dlq"
}

data "aws_sqs_queue" "sqs_notificacao" {
  name = "sqs_notificacao"
}

resource "aws_iam_role" "lambda_execution_dlq_role" {
  name = "lambda_notifica_dlq_execution_role"

  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Action = "sts:AssumeRole"
        Effect = "Allow"
        Principal = {
          Service = "lambda.amazonaws.com"
        }
      },
    ]
  })
}

resource "aws_iam_policy" "lambda_dlq_policy" {
  name        = "lambda_notifica_dlq_policy"
  description = "IAM policy for Lambda execution"
  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Effect = "Allow"
        Action = [
          "logs:CreateLogGroup",
          "logs:CreateLogStream",
          "logs:PutLogEvents",
          "dynamodb:DeleteItem",
          "dynamodb:GetItem",
          "dynamodb:PutItem",
          "dynamodb:Query",
          "dynamodb:Scan",
          "dynamodb:UpdateItem",
          "dynamodb:DescribeTable",
          "sqs:*"
        ]
        Resource = "*"
      }
    ]
  })
}

resource "aws_iam_role_policy_attachment" "lambda_execution_policy" {
  role       = aws_iam_role.lambda_execution_dlq_role.name
  policy_arn = aws_iam_policy.lambda_dlq_policy.arn
}

resource "aws_lambda_function" "lambda_notifica_dlq_function" {
  function_name = "lambda_notifica_dlq_function"
  role          = aws_iam_role.lambda_execution_dlq_role.arn
  runtime       = "dotnet8"
  memory_size   = 512
  timeout       = 30
  handler       = "hackaton_geradorFrame_notifica_dlq::hackaton_geradorFrame_notifica_dlq.Function::FunctionHandler"
  # Codigo armazenado no S3
  s3_bucket = "hackathon-grupo12-fiap-code-bucket"
  s3_key    = "lambda_geradorFrame_notifica.zip"

  environment {
    variables = {
      url_sqs_notificacao = data.aws_sqs_queue.sqs_notificacao.id
    }
  }
}

resource "aws_lambda_permission" "permission_lambda_notifica_dlq" {
  statement_id  = "AllowSQSTrigger"
  action        = "lambda:InvokeFunction"
  function_name = data.aws_lambda_function.lambda_notifica_dlq_function.function_name
  principal     = "sqs.amazonaws.com"
  source_arn    = aws_sqs_queue.sqs_processar_arquivo_dlq.arn
}

resource "aws_lambda_event_source_mapping" "sqs_to_lambda_pagamento_pedido" {
  event_source_arn = aws_sqs_queue.sqs_processar_arquivo_dlq.arn
  function_name    = data.aws_lambda_function.lambda_notifica_dlq_function.function_name
  batch_size       = 10
  enabled          = true
}