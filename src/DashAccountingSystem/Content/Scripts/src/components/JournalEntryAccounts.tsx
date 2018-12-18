import * as React from 'react';
import * as Logging from '../common/logging';
import * as _ from 'lodash';
import { AccountCategoryList, AccountSelector, AccountRecord } from './AccountSelector';
import { AssetTypeRecord, AssetTypeSelector } from './AssetTypeSelector';

interface DashNamespace {
    journalEntry: JournalEntryValidationState
}

interface JournalEntryValidationState {
    error: string,
    hasSufficientAccounts: boolean,
    isBalanced: boolean
}

declare var dash: DashNamespace;

interface JournalEntryAccountRecord {
    accountId: number,
    accountName: string,
    assetTypeId: number,
    assetType: string,
    debit: number,
    credit: number
}

interface JournalEntryAccountsProps {
    accounts: JournalEntryAccountRecord[],
    accountsList: AccountCategoryList[],
    assetTypes: AssetTypeRecord[],
}

interface JournalEntryAccountsState {
    accounts: JournalEntryAccountRecord[],
    addAccountId?: number,
    addAccountName?: string,
    addAssetTypeId?: number,
    addAssetTypeName?: string
    addCredit: number,
    addDebit: number,
    isEntryUnbalanced: boolean
}

interface UpdatedAccountValue {
    credit?: number
    debit?: number
}

export class JournalEntryAccounts extends React.Component<JournalEntryAccountsProps, JournalEntryAccountsState> {
    private logger: Logging.ILogger

    constructor(props: JournalEntryAccountsProps) {
        super(props);
        this.logger = new Logging.Logger('JournalEntryAccounts');

        const { assetTypes } = props;
        const defaultAssetType = assetTypes[0];

        this.state = {
            accounts: props.accounts || [],
            addAssetTypeId: defaultAssetType.id,
            addAssetTypeName: defaultAssetType.name,
            addCredit: 0,
            addDebit: 0,
            isEntryUnbalanced: false
        };

        this._onAccountChange = this._onAccountChange.bind(this);
        this._onAddClick = this._onAddClick.bind(this);
        this._onAddCreditChange = this._onAddCreditChange.bind(this);
        this._onAddDebitChange = this._onAddDebitChange.bind(this);
        this._onAssetTypeChange = this._onAssetTypeChange.bind(this);
        this._onEditCreditChange = this._onEditCreditChange.bind(this);
        this._onEditDebitChange = this._onEditDebitChange.bind(this);
    }

    render() {
        const { accountsList, accounts, assetTypes } = this.props;
        const { accounts: existingAccounts, addAccountId, addAssetTypeId, addCredit, addDebit, isEntryUnbalanced } = this.state;

        const alreadySelectedAccountIds = _.map(existingAccounts, acct => acct.accountId);
        const canAddNewAccount = !_.isNil(addAccountId) &&
            !_.isNil(addAssetTypeId) &&
            ((addCredit || 0) > 0 || (addDebit || 0) > 0);

        return (
            <div>
                <div className="row" style={{ minHeight: '52px' }}>
                    <div className="col-md-10">
                        <h4>Journal Entry Account Details</h4>
                    </div>
                    <div className="col-md-2" style={{ textAlign: 'right' }}>
                        {isEntryUnbalanced ?
                            (
                                <div className="alert alert-warning" style={{ paddingTop: '5px', paddingBottom: '5px' }}>
                                    Entry is not balanced
                                </div>
                            ) : null}
                    </div>
                </div>
                <table className="table" id="accounts-table">
                    <thead>
                        <tr>
                            <th className="col-md-5">Account</th>
                            <th className="col-md-1">Asset Type</th>
                            <th className="col-md-2">Debit</th>
                            <th className="col-md-2">Credit</th>
                            <th className="col-md-2"></th>
                        </tr>
                    </thead>
                    <tbody>
                        {_.map(existingAccounts, (account, index) => {
                            return (
                                <tr key={account.accountId}>
                                    <td className="col-md-5" style={{ verticalAlign: 'middle', paddingLeft: '20px', paddingTop: '9px' }}>
                                        <div>
                                            <input type="hidden" name={`Accounts[${index}].AccountId`} value={account.accountId} />
                                            <input
                                                name={`Accounts[${index}].AccountName`}
                                                readOnly
                                                style={{ border: 'none', width: '100%' }}
                                                type="text"
                                                value={account.accountName}
                                            />
                                        </div>
                                    </td>
                                    <td className="col-md-1">
                                        <div style={{ textAlign: 'right', paddingRight: '11px', paddingTop: '9px' }}>
                                            <input type="hidden" name={`Accounts[${index}].AssetTypeId`} value={account.assetTypeId} />
                                            <input
                                                name={`Accounts[${index}].AssetType`}
                                                readOnly
                                                style={{ border: 'none', textAlign: 'right', width: '100%' }}
                                                type="text"
                                                value={account.assetType}
                                            />
                                        </div>
                                    </td>
                                    <td className="col-md-2">
                                        <input
                                            className="form-control"
                                            min={0}
                                            name={`Accounts[${index}].Debit`}
                                            onChange={(e) => this._onEditDebitChange(e, index)}
                                            step="any"
                                            type="number"
                                            style={{ textAlign: 'right' }}
                                            value={account.debit}
                                        />
                                    </td>
                                    <td className="col-md-2">
                                        <input
                                            className="form-control"
                                            min={0}
                                            name={`Accounts[${index}].Credit`}
                                            onChange={(e) => this._onEditCreditChange(e, index)}
                                            step="any"
                                            type="number"
                                            style={{ textAlign: 'right' }}
                                            value={account.credit}
                                        />
                                    </td>
                                    <td className="col-md-2" style={{ textAlign: 'right' }}>
                                        <button
                                            className="btn btn-danger"
                                            onClick={(e) => this._onRemoveAccount(e, account.accountId)}
                                        >
                                            {'Remove'}
                                        </button>
                                    </td>
                                </tr>
                            );
                        })}
                    </tbody>
                    <tfoot>
                        <tr>
                            <td className="col-md-5">
                                <AccountSelector
                                    accountList={accountsList}
                                    disabledAccountIds={alreadySelectedAccountIds}
                                    id="account-selector"
                                    onChange={this._onAccountChange}
                                    value={addAccountId}
                                />
                            </td>
                            <td className="col-md-1">
                                <AssetTypeSelector
                                    assetTypes={assetTypes}
                                    id="asset-type-selector"
                                    onChange={this._onAssetTypeChange}
                                    value={addAssetTypeId}
                                />
                            </td>
                            <td className="col-md-2">
                                <input
                                    className="form-control"
                                    id="add-debit-amount"
                                    min={0}
                                    onChange={this._onAddDebitChange}
                                    step="any"
                                    type="number"
                                    style={{ textAlign: 'right' }}
                                    value={addDebit}
                                />
                            </td>
                            <td className="col-md-2">
                                <input
                                    className="form-control"
                                    id="add-credit-amount"
                                    min={0}
                                    onChange={this._onAddCreditChange}
                                    step="any"
                                    style={{ textAlign: 'right' }}
                                    type="number"
                                    value={addCredit}
                                 />
                            </td>
                            <td className="col-md-2" style={{ textAlign: 'right' }}>
                                <button
                                    className="btn btn-success"
                                    disabled={!canAddNewAccount}
                                    id="add-account-button"
                                    onClick={this._onAddClick}
                                >
                                    {'Add'}
                                </button>
                            </td>
                        </tr>
                    </tfoot>
                </table>
            </div>
        );
    }

    _onAddClick(event: React.MouseEvent<HTMLElement>) {
        event.preventDefault();
        const { accounts, addAccountId, addAccountName, addAssetTypeId, addAssetTypeName, addDebit, addCredit } = this.state;
        const newAccount = {
            accountId: addAccountId,
            accountName: addAccountName,
            assetTypeId: addAssetTypeId,
            assetType: addAssetTypeName,
            credit: addCredit || 0,
            debit: addDebit || 0
        };

        this.setState({
            accounts: _.concat(accounts, newAccount),
            addAccountId: null,
            addAccountName: null,
            addCredit: 0,
            addDebit: 0
        }, () => {
            this._updateValidationState();
        });

        const $accountSelector = $('#account-selector');
        $accountSelector.val('');
        $accountSelector.selectpicker('refresh');
    }

    _onAddCreditChange(event: React.ChangeEvent<HTMLInputElement>) {
        const newValue = event.target.value;
        this.setState({ addCredit: _.isNil(newValue) ? null : parseFloat(newValue) });
    }

    _onAddDebitChange(event: React.ChangeEvent<HTMLInputElement>) {
        const newValue = event.target.value;
        this.setState({ addDebit: _.isNil(newValue) ? null : parseFloat(newValue) });
    }

    _onAccountChange(selectedAccount: AccountRecord) {
        if (!_.isNil(selectedAccount)) {
            this.setState({ addAccountId: selectedAccount.id, addAccountName: selectedAccount.name });
        } else {
            this.setState({ addAccountId: null, addAccountName: null });
        }
    }

    _onAssetTypeChange(selectedAssetType: AssetTypeRecord) {
        this.setState({ addAssetTypeId: selectedAssetType.id, addAssetTypeName: selectedAssetType.name });
    }

    _onEditCreditChange(event: React.ChangeEvent<HTMLInputElement>, index: number) {
        let newCreditValue = parseFloat(event.target.value);

        if (_.isNaN(newCreditValue))
            newCreditValue = 0;

        const newCreditValueObj = { credit: newCreditValue };
        this._updateAccountAmount(index, newCreditValueObj);
    }

    _onEditDebitChange(event: React.ChangeEvent<HTMLInputElement>, index: number) {
        let newDebitValue = parseFloat(event.target.value);

        if (_.isNaN(newDebitValue)) {
            newDebitValue = 0;
        }

        const newDebitValueObj = { debit: newDebitValue };
        this._updateAccountAmount(index, newDebitValueObj);
    }

    _onRemoveAccount(event: React.MouseEvent<HTMLElement>, accountId: number) {
        event.preventDefault();
        const { accounts } = this.state;
        this.setState({
            accounts: _.filter(accounts, a => a.accountId !== accountId)
        }, () => {
            this._updateValidationState();
        });
    }

    _updateAccountAmount(index: number, newAmountValue: UpdatedAccountValue) {
        const { accounts } = this.state;
        const affectedAccount = accounts[index];
        const updatedAccount = _.assign({}, affectedAccount, newAmountValue);
        const updatedAccountEntry: any = {};
        updatedAccountEntry[index] = updatedAccount;
        const updatedAccountsCollection = _.assign([], accounts, updatedAccountEntry);
        this.setState({ accounts: updatedAccountsCollection }, () => {
            this._updateValidationState();
        });
    }

    _updateValidationState() {
        const { accounts } = this.state;
        let errorMessage: string | null = null;
        let isEntryBalanced: boolean | null = null;
        const hasSufficientAccounts = accounts.length >= 2;

        if (!hasSufficientAccounts) {
            errorMessage = accounts.length === 0
                ? 'Journal Entry does not have any accounts'
                : 'Journal Entry must have at least two accounts';
        }

        const groupedByAssetType = _.groupBy(accounts, a => a.assetType);
        isEntryBalanced = _.every(groupedByAssetType, (grp, assetType) => {
            const debits = _.sumBy(grp, a => a.debit);
            const credits = _.sumBy(grp, a => a.credit);
            const isGroupBalanced = debits === credits;

            if (hasSufficientAccounts && !isGroupBalanced)
                errorMessage = `Amounts of type ${assetType} do not balance`;

            return isGroupBalanced;
        });

        dash.journalEntry = _.assign({}, dash.journalEntry, {
            error: errorMessage || '',
            isBalanced: isEntryBalanced,
            hasSufficientAccounts
        });

        this.setState({ isEntryUnbalanced: hasSufficientAccounts && !isEntryBalanced });
    }
}
