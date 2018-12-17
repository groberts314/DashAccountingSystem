import * as React from 'react';
import * as _ from 'lodash';

export interface AssetTypeRecord {
    id: number,
    name: string
}

interface AssetTypeSelectorProps {
    assetTypes: AssetTypeRecord[],
    id: string,
    onChange: Function,
    value?: number
}

export class AssetTypeSelector extends React.PureComponent<AssetTypeSelectorProps> {
    constructor(props: AssetTypeSelectorProps) {
        super(props);
        this._onChange = this._onChange.bind(this);
    }

    render() {
        const { assetTypes, id, value } = this.props;

        return (
            <select
                className="selectpicker form-control"
                data-dropup-auto="false"
                data-width="auto"
                id={id}
                onChange={this._onChange}
                value={!_.isNil(value) ? value : ''}
            >
                {_.map(assetTypes, at => {
                    const assetTypeAbbreviation = at.name.split(' ')[0];
                    return (
                        <option
                            data-icon={`glyphicon-${assetTypeAbbreviation.toLowerCase()}`}
                            data-label={at.name}
                            key={at.id}
                            value={at.id}
                        >
                            {assetTypeAbbreviation}
                        </option>
                    )
                })}
            </select>
        );
    }

    _onChange(event: React.ChangeEvent<HTMLSelectElement>) {
        const { assetTypes, onChange } = this.props;
        const selectElement = event.target;

        if (selectElement.selectedIndex === -1)
            onChange(null);

        const selectedOption = selectElement.selectedOptions[0];
        const selectedAssetType = _.find(assetTypes, at => at.id === parseInt(selectedOption.value));

        onChange(selectedAssetType);
    }
}