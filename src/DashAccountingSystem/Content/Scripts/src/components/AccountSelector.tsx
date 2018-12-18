import * as React from 'react';
import * as _ from 'lodash';

export interface AccountRecord {
    id: number,
    name: string
}

export interface AccountCategoryList {
    category: string,
    accounts: AccountRecord[]
}

interface AccountSelectorProps {
    accountList: AccountCategoryList[],
    disabledAccountIds: number[],
    id: string,
    onChange: Function
    value?: number
}

export class AccountSelector extends React.PureComponent<AccountSelectorProps> {
    constructor(props: AccountSelectorProps) {
        super(props);
        this._onChange = this._onChange.bind(this);
    }

    render() {
        const { accountList, disabledAccountIds, id, value } = this.props;

        return (
            <select
                className="selectpicker form-control"
                data-size="10"
                id={id}
                onChange={this._onChange}
                title="Select an Account"
                value={!_.isNil(value) ? value : ''}
            >
                {_.map(accountList, acctCategoryGroup => {
                    return (
                        <optgroup key={ _.kebabCase(acctCategoryGroup.category) } label={acctCategoryGroup.category}>
                            {_.map(acctCategoryGroup.accounts, account => {
                                const isDisabled = _.includes(disabledAccountIds, account.id);

                                return (
                                    <option
                                        disabled={isDisabled}
                                        key={account.id}
                                        value={account.id}
                                    >
                                        {account.name}
                                    </option>
                                );
                            })}
                        </optgroup>
                    );
                })}
            </select>
        );
    }

    _onChange(event: React.ChangeEvent<HTMLSelectElement>) {
        const { onChange } = this.props;
        const selectElement = event.target;

        if (selectElement.selectedIndex === -1)
            onChange(null);

        const selectedOption = selectElement.selectedOptions[0];

        onChange({ id: parseInt(selectedOption.value), name: selectedOption.label });
    }
}
