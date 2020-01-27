﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CodeStyle;
using Microsoft.CodeAnalysis.CSharp.CodeStyle;
using Microsoft.CodeAnalysis.CSharp.UseExpressionBody;
using Microsoft.CodeAnalysis.Editor.CSharp.UnitTests.CodeRefactorings;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.Test.Utilities;
using Roslyn.Test.Utilities;
using Xunit;

namespace Microsoft.CodeAnalysis.Editor.CSharp.UnitTests.UseExpressionBody
{
    public class UseExpressionBodyForAccessorsRefactoringTests : AbstractCSharpCodeActionTest
    {
        protected override CodeRefactoringProvider CreateCodeRefactoringProvider(Workspace workspace, TestParameters parameters)
            => new UseExpressionBodyCodeRefactoringProvider();

        private IDictionary<OptionKey, object> UseExpressionBodyForAccessors_BlockBodyForProperties =>
            OptionsSet(
                this.SingleOption(CSharpCodeStyleOptions.PreferExpressionBodiedAccessors, CSharpCodeStyleOptions.WhenPossibleWithSilentEnforcement),
                this.SingleOption(CSharpCodeStyleOptions.PreferExpressionBodiedProperties, CSharpCodeStyleOptions.NeverWithSilentEnforcement));

        private IDictionary<OptionKey, object> UseExpressionBodyForAccessors_BlockBodyForProperties_DisabledDiagnostic =>
            OptionsSet(
                this.SingleOption(CSharpCodeStyleOptions.PreferExpressionBodiedAccessors, new CodeStyleOption<ExpressionBodyPreference>(ExpressionBodyPreference.WhenPossible, NotificationOption.None)),
                this.SingleOption(CSharpCodeStyleOptions.PreferExpressionBodiedProperties, new CodeStyleOption<ExpressionBodyPreference>(ExpressionBodyPreference.Never, NotificationOption.None)));

        private IDictionary<OptionKey, object> UseExpressionBodyForAccessors_ExpressionBodyForProperties =>
            OptionsSet(
                this.SingleOption(CSharpCodeStyleOptions.PreferExpressionBodiedAccessors, CSharpCodeStyleOptions.WhenPossibleWithSilentEnforcement),
                this.SingleOption(CSharpCodeStyleOptions.PreferExpressionBodiedProperties, CSharpCodeStyleOptions.WhenPossibleWithSilentEnforcement));

        private IDictionary<OptionKey, object> UseExpressionBodyForAccessors_ExpressionBodyForProperties_DisabledDiagnostic =>
            OptionsSet(
                this.SingleOption(CSharpCodeStyleOptions.PreferExpressionBodiedAccessors, new CodeStyleOption<ExpressionBodyPreference>(ExpressionBodyPreference.WhenPossible, NotificationOption.None)),
                this.SingleOption(CSharpCodeStyleOptions.PreferExpressionBodiedProperties, new CodeStyleOption<ExpressionBodyPreference>(ExpressionBodyPreference.WhenPossible, NotificationOption.None)));

        private IDictionary<OptionKey, object> UseBlockBodyForAccessors_ExpressionBodyForProperties =>
            OptionsSet(
                this.SingleOption(CSharpCodeStyleOptions.PreferExpressionBodiedAccessors, CSharpCodeStyleOptions.NeverWithSilentEnforcement),
                this.SingleOption(CSharpCodeStyleOptions.PreferExpressionBodiedProperties, CSharpCodeStyleOptions.WhenPossibleWithSilentEnforcement));

        private IDictionary<OptionKey, object> UseBlockBodyForAccessors_ExpressionBodyForProperties_DisabledDiagnostic =>
            OptionsSet(
                this.SingleOption(CSharpCodeStyleOptions.PreferExpressionBodiedAccessors, new CodeStyleOption<ExpressionBodyPreference>(ExpressionBodyPreference.Never, NotificationOption.None)),
                this.SingleOption(CSharpCodeStyleOptions.PreferExpressionBodiedProperties, new CodeStyleOption<ExpressionBodyPreference>(ExpressionBodyPreference.WhenPossible, NotificationOption.None)));

        private IDictionary<OptionKey, object> UseBlockBodyForAccessors_BlockBodyForProperties =>
            OptionsSet(
                this.SingleOption(CSharpCodeStyleOptions.PreferExpressionBodiedAccessors, CSharpCodeStyleOptions.NeverWithSilentEnforcement),
                this.SingleOption(CSharpCodeStyleOptions.PreferExpressionBodiedProperties, CSharpCodeStyleOptions.NeverWithSilentEnforcement));

        private IDictionary<OptionKey, object> UseBlockBodyForAccessors_BlockBodyForProperties_DisabledDiagnostic =>
            OptionsSet(
                this.SingleOption(CSharpCodeStyleOptions.PreferExpressionBodiedAccessors, new CodeStyleOption<ExpressionBodyPreference>(ExpressionBodyPreference.Never, NotificationOption.None)),
                this.SingleOption(CSharpCodeStyleOptions.PreferExpressionBodiedProperties, new CodeStyleOption<ExpressionBodyPreference>(ExpressionBodyPreference.Never, NotificationOption.None)));

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsUseExpressionBody)]
        public async Task TestUpdatePropertyIfPropertyWantsBlockAndAccessorWantsExpression()
        {
            await TestInRegularAndScript1Async(
@"class C
{
    int Goo
    {
        get
        {
            [||]return Bar();
        }
    }
}",
@"class C
{
    int Goo => Bar();
}", parameters: new TestParameters(options: UseExpressionBodyForAccessors_BlockBodyForProperties));
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsUseExpressionBody)]
        public async Task TestNotOfferedIfUserPrefersExpressionBodiesAndInBlockBody2()
        {
            await TestMissingAsync(
@"class C
{
    int Goo
    {
        get
        {
            [||]return Bar();
        }
    }
}", parameters: new TestParameters(options: UseExpressionBodyForAccessors_ExpressionBodyForProperties));
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsUseExpressionBody)]
        public async Task TestOfferedIfUserPrefersExpressionBodiesWithoutDiagnosticAndInBlockBody()
        {
            await TestInRegularAndScript1Async(
@"class C
{
    int Goo
    {
        get
        {
            return [||]Bar();
        }
    }
}",
@"class C
{
    int Goo => Bar();
}", parameters: new TestParameters(options: UseExpressionBodyForAccessors_BlockBodyForProperties_DisabledDiagnostic));
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsUseExpressionBody)]
        public async Task TestOfferedIfUserPrefersExpressionBodiesWithoutDiagnosticAndInBlockBody2()
        {
            await TestInRegularAndScript1Async(
@"class C
{
    int Goo
    {
        get
        {
            return [||]Bar();
        }
    }
}",
@"class C
{
    int Goo => Bar();
}", parameters: new TestParameters(options: UseExpressionBodyForAccessors_ExpressionBodyForProperties_DisabledDiagnostic));
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsUseExpressionBody)]
        public async Task TestOfferedIfUserPrefersBlockBodiesAndInBlockBody()
        {
            await TestInRegularAndScript1Async(
@"class C
{
    int Goo
    {
        get
        {
            return [||]Bar();
        }
    }
}",
@"class C
{
    int Goo
    {
        get => Bar();
    }
}", parameters: new TestParameters(options: UseBlockBodyForAccessors_ExpressionBodyForProperties));
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsUseExpressionBody)]
        public async Task TestOfferExpressionBodyForPropertyIfPropertyAndAccessorBothPreferExpressions()
        {
            await TestInRegularAndScript1Async(
@"class C
{
    int Goo
    {
        get
        {
            return [||]Bar();
        }
    }
}",
@"class C
{
    int Goo => [||]Bar();
}", parameters: new TestParameters(options: UseBlockBodyForAccessors_BlockBodyForProperties));
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsUseExpressionBody)]
        public async Task TestNotOfferedIfUserPrefersBlockBodiesAndInExpressionBody()
        {
            await TestMissingAsync(
@"class C
{
    int Goo { get => [||]Bar(); }
}", parameters: new TestParameters(options: UseBlockBodyForAccessors_ExpressionBodyForProperties));
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsUseExpressionBody)]
        public async Task TestOfferedIfUserPrefersBlockBodiesWithoutDiagnosticAndInExpressionBody()
        {
            await TestInRegularAndScript1Async(
@"class C
{
    int Goo { get => [||]Bar(); }
}",

@"class C
{
    int Goo => Bar();
}", parameters: new TestParameters(options: UseBlockBodyForAccessors_BlockBodyForProperties_DisabledDiagnostic));
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsUseExpressionBody)]
        public async Task TestOfferedIfUserPrefersBlockBodiesWithoutDiagnosticAndInExpressionBody2()
        {
            await TestInRegularAndScript1Async(
@"class C
{
    int Goo { get => [||]Bar(); }
}",

@"class C
{
    int Goo => Bar();
}", parameters: new TestParameters(options: UseBlockBodyForAccessors_ExpressionBodyForProperties_DisabledDiagnostic));
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsUseExpressionBody)]
        public async Task TestOfferedForPropertyIfUserPrefersBlockPropertiesAndHasBlockProperty()
        {
            await TestInRegularAndScript1Async(
@"class C
{
    int Goo { get => [||]Bar(); }
}",

@"class C
{
    int Goo => Bar();
}", parameters: new TestParameters(options: UseBlockBodyForAccessors_BlockBodyForProperties));
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsUseExpressionBody)]
        public async Task TestOfferForPropertyIfPropertyPrefersBlockButCouldBecomeExpressionBody()
        {
            await TestInRegularAndScript1Async(
@"class C
{
    int Goo { get => [||]Bar(); }
}",
@"class C
{
    int Goo => Bar();
}", parameters: new TestParameters(options: UseExpressionBodyForAccessors_BlockBodyForProperties));
        }

        [WorkItem(20350, "https://github.com/dotnet/roslyn/issues/20350")]
        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsUseExpressionBody)]
        public async Task TestAccessorListFormatting()
        {
            await TestInRegularAndScript1Async(
@"class C
{
    int Goo { get => [||]Bar(); }
}",
@"class C
{
    int Goo
    {
        get
        {
            return Bar();
        }
    }
}", parameters: new TestParameters(options: UseExpressionBodyForAccessors_ExpressionBodyForProperties));
        }
    }
}
