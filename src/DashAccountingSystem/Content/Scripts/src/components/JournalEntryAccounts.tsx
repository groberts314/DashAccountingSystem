import * as React from 'react';
import * as Logging from '../common/logging';
import * as _ from 'lodash';
import { HtmlInputType } from './formutils';
import { AccountCategoryList, AccountSelector, AccountRecord } from './AccountSelector';
 
interface AssetTypeRecord {
    id: number,
    name: string
}

interface JournalEntryAccountRecord {
    accountId: number,
    accountName: string,
    assetTypeId: number,
    assetTypeName: string,
    debit: number,
    credit: number
}

interface JournalEntryAccountsProps {
    accountsList: AccountCategoryList[],
    assetTypes: AssetTypeRecord[],
    accounts: JournalEntryAccountRecord[]
}

interface JournalEntryAccountsState {
    accounts: JournalEntryAccountRecord[],
    addAccountId?: number,
    addAccountName?: string,
    addAssetTypeId?: number,
    addAssetTypeName?: string
}

export class JournalEntryAccounts extends React.Component<JournalEntryAccountsProps, JournalEntryAccountsState> {
    private logger: Logging.ILogger

    constructor(props: JournalEntryAccountsProps) {
        super(props);
        this.logger = new Logging.Logger('JournalEntryAccounts');
        this.logger.info('Lodash Version:', _.VERSION);

        this.state = {
            accounts: props.accounts || []
        };

        this._onAccountChange = this._onAccountChange.bind(this);
        this._onAddClick = this._onAddClick.bind(this);
    }

    render() {
        const { accountsList, accounts } = this.props;
        const { accounts: existingAccounts, addAccountId } = this.state;

        const alreadySelectedAccountIds = _.map(existingAccounts, acct => acct.accountId);

        return (
            <table className="table" id="accounts-table">
                <thead>
                    <tr>
                        <th className="col-md-5">Account</th>
                        <th className="col-md-2">Asset Type</th>
                        <th className="col-md-2">Debit</th>
                        <th className="col-md-2">Credit</th>
                        <th className="col-md-1"></th>
                    </tr>
                </thead>
                <tbody>
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
                        <td className="col-md-2"></td>
                        <td className="col-md-2"></td>
                        <td className="col-md-2"></td>
                        <td className="col-md-1"></td>
                    </tr>
                </tfoot>
            </table>
        );
    }

    _onAddClick() {

    }

    _onAccountChange(selectedAccount: AccountRecord) {
        this.logger.info('Account Changed!  Selected:', selectedAccount);

        if (!_.isNil(selectedAccount)) {
            this.setState({ addAccountId: selectedAccount.id, addAccountName: selectedAccount.name });
        } else {
            this.setState({ addAccountId: null, addAccountName: null });
        }
    }
}
