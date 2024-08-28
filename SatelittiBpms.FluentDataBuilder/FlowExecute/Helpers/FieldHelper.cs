using SatelittiBpms.Models.Enums;
using System;

namespace SatelittiBpms.FluentDataBuilder.FlowExecute.Helpers
{
    public static class FieldHelper
    {
        private static readonly Bogus.Faker _faker = new();

        public static object GenerateValueByType(FieldTypeEnum type)
        {
            return type switch
            {
                FieldTypeEnum.TEXTFIELD => _faker.Random.Words(),
                FieldTypeEnum.TEXTAREA => _faker.Lorem.Paragraphs(),
                FieldTypeEnum.EMAIL => _faker.Person.Email,
                FieldTypeEnum.NUMBER => _faker.Random.Number(int.MinValue, int.MaxValue),
                FieldTypeEnum.CURRENCY => _faker.Random.Number(int.MinValue, int.MaxValue),
                FieldTypeEnum.DATETIME => _faker.Date.Between(DateTime.MinValue, DateTime.MaxValue).ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
                FieldTypeEnum.CHECKBOX => _faker.Random.Bool(),
                FieldTypeEnum.SELECT => throw new ArgumentOutOfRangeException(nameof(type), type, "Não foi implementado para gerar o valor para o tipo de campo."),
                // TODO Falta colocar para gerar valores para esses campos e colocar o gerador de campos para gerar as propriedades deles corretamente
                FieldTypeEnum.RADIO => throw new ArgumentOutOfRangeException(nameof(type), type, "Não foi implementado para gerar o valor para o tipo de campo."),
                FieldTypeEnum.COLUMNS => null,
                FieldTypeEnum.TABS => null,
                FieldTypeEnum.TABLE => null,
                FieldTypeEnum.FILE => throw new ArgumentOutOfRangeException(nameof(type), type, "Não é possível gerar dados para o tipo de arquivo deve inserir no banco."),
                FieldTypeEnum.CONTENT => null,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Não foi implementado para gerar o valor para o tipo de campo."),
            };
        }
    }
}
